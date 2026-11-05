using System.Net;
using System.Text.RegularExpressions;

namespace CineHollywoodTPI.API.Services
{
    // Guarda los pósters de las películas en wwwroot/posters de la API.
    // En la base se guarda la ruta relativa: "/posters/archivo.jpg"
    public class PosterService
    {
        public const string CarpetaPublica = "/posters/";
        private const long TamanioMaximo = 5 * 1024 * 1024; // 5 MB

        private readonly string _carpeta;
        private readonly IHttpClientFactory _httpFactory;

        public PosterService(IWebHostEnvironment env, IHttpClientFactory httpFactory)
        {
            _carpeta = Path.Combine(env.ContentRootPath, "wwwroot", "posters");
            Directory.CreateDirectory(_carpeta);
            _httpFactory = httpFactory;
        }

        // 1) Imagen subida desde la PC (multipart/form-data)
        public async Task<string> GuardarArchivoAsync(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                throw new NegocioException("No se recibió ninguna imagen.");
            if (archivo.Length > TamanioMaximo)
                throw new NegocioException("La imagen no puede superar los 5 MB.");

            using var ms = new MemoryStream();
            await archivo.CopyToAsync(ms);
            return await GuardarBytesAsync(ms.ToArray());
        }

        // 2) Link: se descarga en el servidor. Si el link es una página web,
        //    se busca la imagen principal de la página (meta og:image).
        public async Task<string> DescargarDesdeUrlAsync(string url)
        {
            if (!Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new NegocioException("El link no es válido (tiene que empezar con http:// o https://).");

            var (bytes, contentType) = await DescargarAsync(uri);

            if (contentType.StartsWith("text/html"))
            {
                var html = System.Text.Encoding.UTF8.GetString(bytes);
                var imagen = BuscarImagenPrincipal(html, uri)
                    ?? throw new NegocioException(
                        "El link es una página web y no se encontró una imagen en ella. Usá el link directo de la imagen (clic derecho sobre la imagen → \"Copiar dirección de imagen\").");

                (bytes, contentType) = await DescargarAsync(imagen);
            }

            return await GuardarBytesAsync(bytes);
        }

        // Borra un póster anterior guardado por nosotros (no toca links externos)
        public void BorrarSiEsLocal(string? posterUrl)
        {
            if (string.IsNullOrEmpty(posterUrl) || !posterUrl.StartsWith(CarpetaPublica)) return;
            var ruta = Path.Combine(_carpeta, Path.GetFileName(posterUrl));
            if (File.Exists(ruta)) File.Delete(ruta);
        }

        private async Task<(byte[] bytes, string contentType)> DescargarAsync(Uri uri)
        {
            var http = _httpFactory.CreateClient("posters");
            using var resp = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            if (!resp.IsSuccessStatusCode)
                throw new NegocioException($"No se pudo descargar el link (el sitio respondió {(int)resp.StatusCode}).");

            if (resp.Content.Headers.ContentLength > TamanioMaximo)
                throw new NegocioException("La imagen no puede superar los 5 MB.");

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            if (bytes.Length > TamanioMaximo)
                throw new NegocioException("La imagen no puede superar los 5 MB.");

            var tipo = resp.Content.Headers.ContentType?.MediaType?.ToLowerInvariant() ?? "";
            return (bytes, tipo);
        }

        private static Uri? BuscarImagenPrincipal(string html, Uri paginaBase)
        {
            // <meta property="og:image" content="..."> (también twitter:image), en cualquier orden de atributos
            var patrones = new[]
            {
                @"<meta[^>]+(?:property|name)\s*=\s*[""'](?:og:image(?::secure_url)?|twitter:image)[""'][^>]*content\s*=\s*[""']([^""']+)[""']",
                @"<meta[^>]+content\s*=\s*[""']([^""']+)[""'][^>]*(?:property|name)\s*=\s*[""'](?:og:image(?::secure_url)?|twitter:image)[""']"
            };

            foreach (var patron in patrones)
            {
                var m = Regex.Match(html, patron, RegexOptions.IgnoreCase);
                if (m.Success && Uri.TryCreate(paginaBase, WebUtility.HtmlDecode(m.Groups[1].Value), out var img))
                    return img;
            }
            return null;
        }

        private async Task<string> GuardarBytesAsync(byte[] bytes)
        {
            // Verificamos que realmente sea una imagen mirando los primeros bytes (no la extensión)
            var extension = DetectarExtension(bytes)
                ?? throw new NegocioException("El archivo no es una imagen válida (se aceptan JPG, PNG, WEBP o GIF).");

            var nombre = $"{Guid.NewGuid():N}{extension}";
            await File.WriteAllBytesAsync(Path.Combine(_carpeta, nombre), bytes);
            return CarpetaPublica + nombre;
        }

        private static string? DetectarExtension(byte[] b)
        {
            if (b.Length < 12) return null;
            if (b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF) return ".jpg";
            if (b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return ".png";
            if (b[0] == 0x47 && b[1] == 0x49 && b[2] == 0x46) return ".gif";
            if (b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46 &&
                b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50) return ".webp";
            return null;
        }
    }
}
