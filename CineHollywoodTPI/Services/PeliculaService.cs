using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de películas:
    //  - Título obligatorio (máx. 200), duración entre 1 y 600 minutos, clasificación existente.
    //  - Baja lógica (activo = 0).
    //  - Cartelera = activas con funciones desde hoy; estrenos = activas sin funciones.
    //  - El póster se guarda en el servidor (subido o descargado desde un link).
    public class PeliculaService : IPeliculaService
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly PosterService _posterService;

        public PeliculaService(IPeliculaRepository peliculaRepository, ICatalogoRepository catalogoRepository, PosterService posterService)
        {
            _peliculaRepository = peliculaRepository;
            _catalogoRepository = catalogoRepository;
            _posterService = posterService;
        }

        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        private static PeliculaDTO ToDto(Pelicula p) => new()
        {
            IdPelicula = p.IdPelicula,
            Titulo = p.Titulo ?? "",
            DuracionMinutos = p.DuracionMinutos,
            IdClasificacion = p.IdClasificacion,
            ClasificacionDescripcion = p.IdClasificacionNavigation?.Descripcion,
            PosterUrl = p.PosterUrl,
            Activo = p.Activo
        };

        private async Task ValidarAsync(string? titulo, int? duracion, int? idClasificacion)
        {
            if (string.IsNullOrWhiteSpace(titulo)) throw new NegocioException("El título es obligatorio.");
            if (titulo.Trim().Length > 200) throw new NegocioException("El título no puede superar los 200 caracteres.");
            if (duracion is null or <= 0 or > 600) throw new NegocioException("La duración tiene que estar entre 1 y 600 minutos.");

            if (idClasificacion.HasValue &&
                !(await _catalogoRepository.GetClasificacionesAsync()).Any(c => c.Id == idClasificacion))
                throw new NegocioException("La clasificación seleccionada no existe.");
        }

        private async Task<Pelicula> GetActivaOFallarAsync(int id)
        {
            var p = await _peliculaRepository.GetByIdAsync(id);
            if (p == null || !p.Activo)
                throw new NoEncontradoException($"No se encontró ninguna película activa con el ID {id}.");
            return p;
        }

        // ===================== Consultas =====================

        public async Task<IEnumerable<PeliculaDTO>> GetActivasAsync() =>
            (await _peliculaRepository.GetAllActiveAsync()).Select(ToDto).ToList();

        public async Task<IEnumerable<PeliculaDTO>> GetCarteleraAsync() =>
            (await _peliculaRepository.GetEnCarteleraAsync(Hoy)).Select(ToDto).ToList();

        public async Task<IEnumerable<PeliculaDTO>> GetEstrenosAsync() =>
            (await _peliculaRepository.GetProximosEstrenosAsync(Hoy)).Select(ToDto).ToList();

        public async Task<IEnumerable<PeliculaDTO>> BuscarAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new NegocioException("El parámetro de búsqueda 'nombre' no puede estar vacío.");
            return (await _peliculaRepository.GetByNombreAsync(nombre.Trim())).Select(ToDto).ToList();
        }

        public async Task<PeliculaDTO> GetPorIdAsync(int id) => ToDto(await GetActivaOFallarAsync(id));

        // ===================== ABM =====================

        public async Task<int> CrearAsync(PeliculaInsertDTO dto)
        {
            if (dto == null) throw new NegocioException("Los datos enviados para la nueva película son inválidos.");
            await ValidarAsync(dto.Titulo, dto.DuracionMinutos, dto.IdClasificacion);

            var nueva = new Pelicula
            {
                Titulo = dto.Titulo.Trim(),
                DuracionMinutos = dto.DuracionMinutos,
                IdClasificacion = dto.IdClasificacion,
                PosterUrl = string.IsNullOrWhiteSpace(dto.PosterUrl) ? null : dto.PosterUrl.Trim()
            };

            await _peliculaRepository.AddAsync(nueva);
            return nueva.IdPelicula;
        }

        public async Task ModificarAsync(int id, PeliculaUpdateDTO dto)
        {
            if (dto == null || id != dto.IdPelicula)
                throw new NegocioException("Los identificadores de la petición no coinciden.");
            await ValidarAsync(dto.Titulo, dto.DuracionMinutos, dto.IdClasificacion);

            var pelicula = await _peliculaRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No se encontró ninguna película con el ID {id}.");

            var posterAnterior = pelicula.PosterUrl;
            pelicula.Titulo = dto.Titulo.Trim();
            pelicula.DuracionMinutos = dto.DuracionMinutos;
            pelicula.IdClasificacion = dto.IdClasificacion;
            pelicula.PosterUrl = string.IsNullOrWhiteSpace(dto.PosterUrl) ? null : dto.PosterUrl.Trim();

            await _peliculaRepository.UpdateAsync(pelicula);

            if (posterAnterior != pelicula.PosterUrl)
                _posterService.BorrarSiEsLocal(posterAnterior);
        }

        public async Task DarDeBajaAsync(int id)
        {
            if (!await _peliculaRepository.DeleteLogicoAsync(id))
                throw new NoEncontradoException($"No existe la película con ID {id} o ya está dada de baja.");
        }

        // ===================== Póster =====================

        public Task<string> SubirPosterAsync(int id, IFormFile archivo) =>
            ReemplazarPosterAsync(id, () => _posterService.GuardarArchivoAsync(archivo));

        public Task<string> DescargarPosterAsync(int id, string url) =>
            ReemplazarPosterAsync(id, () => _posterService.DescargarDesdeUrlAsync(url));

        private async Task<string> ReemplazarPosterAsync(int id, Func<Task<string>> guardar)
        {
            var pelicula = await GetActivaOFallarAsync(id);

            string nuevo;
            try
            {
                nuevo = await guardar();
            }
            catch (HttpRequestException)
            {
                throw new NegocioException("No se pudo descargar la imagen desde ese link. Probá descargarla y subir el archivo.");
            }
            catch (TaskCanceledException)
            {
                throw new NegocioException("El sitio de la imagen tardó demasiado en responder. Probá descargarla y subir el archivo.");
            }

            var anterior = pelicula.PosterUrl;
            pelicula.PosterUrl = nuevo;
            await _peliculaRepository.UpdateAsync(pelicula);
            _posterService.BorrarSiEsLocal(anterior);
            return nuevo;
        }
    }
}
