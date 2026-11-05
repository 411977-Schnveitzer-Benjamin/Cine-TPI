using System.Text.RegularExpressions;
using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de autenticación:
    //  - Login: email + contraseña verificada con hash (PBKDF2). Mismo mensaje si el email no existe
    //    o la contraseña es incorrecta. El usuario tiene que estar Activo.
    //  - Registro: datos obligatorios, DNI numérico, email válido y no repetido, contraseña de 6+ caracteres.
    //    Si ya existe una persona con ese DNI (ej. cargada en boletería) se reutiliza, salvo que ya tenga cuenta.
    public class AuthService : IAuthService
    {
        public const string RolAdministrador = "Administrador";
        public const string RolCliente = "Cliente";
        private const int EstadoActivo = 1;

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthService(IUsuarioRepository usuarioRepository, TokenService tokenService, IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginRespuestaDTO> LoginAsync(LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new NegocioException("Ingresá el email y la contraseña.");

            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if (usuario == null || !PasswordValida(usuario, dto.Password))
                throw new NoAutorizadoException("Email o contraseña incorrectos.");

            if (usuario.IdEstadoUsuario != EstadoActivo)
                throw new NoAutorizadoException("El usuario está inactivo.");

            return _tokenService.GenerarToken(usuario);
        }

        public async Task<LoginRespuestaDTO> RegistrarAsync(RegistroDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.NroDocumento) ||
                string.IsNullOrWhiteSpace(dto.Email))
                throw new NegocioException("Completá todos los campos obligatorios.");

            var dni = dto.NroDocumento.Trim();
            if (dni.Length < 6 || !dni.All(char.IsDigit))
                throw new NegocioException("El DNI tiene que tener al menos 6 dígitos (solo números).");
            if (!Regex.IsMatch(dto.Email.Trim(), @"^\S+@\S+\.\S+$"))
                throw new NegocioException("El email no es válido.");
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                throw new NegocioException("La contraseña tiene que tener al menos 6 caracteres.");
            if (dto.FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
                throw new NegocioException("La fecha de nacimiento no puede ser futura.");

            if (await _usuarioRepository.EmailExisteAsync(dto.Email))
                throw new ConflictoException("Ya existe una cuenta con ese email.");

            var idRolCliente = await _usuarioRepository.GetIdRolAsync(RolCliente)
                ?? throw new Exception($"No existe el rol '{RolCliente}' en la tabla ROL_USUARIO.");

            // ¿La persona ya existe (por DNI)? Ej: un cliente cargado antes en boletería
            var persona = await _usuarioRepository.GetPersonaPorDniAsync(dni);
            if (persona?.Usuario != null)
                throw new ConflictoException("Ya existe una cuenta registrada con ese DNI.");

            if (persona == null)
            {
                persona = new Persona { NroDocumento = dni };
            }

            persona.Nombre = dto.Nombre.Trim();
            persona.Apellido = dto.Apellido.Trim();
            persona.FechaNacimiento ??= dto.FechaNacimiento;

            if (persona.Cliente == null)
                persona.Cliente = new Cliente { Activo = true, EsJubilado = false };
            else
                persona.Cliente.Activo = true;

            var email = dto.Email.Trim().ToLower();
            await _usuarioRepository.AgregarAsync(new Usuario
            {
                IdPersonaNavigation = persona,
                IdRol = idRolCliente,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(null!, dto.Password),
                IdEstadoUsuario = EstadoActivo,
                FechaRegistro = DateTime.Now
            });

            // Queda logueado: devolvemos el token
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            return _tokenService.GenerarToken(usuario!);
        }

        public async Task<bool> CrearAdminInicialAsync(string email, string password)
        {
            var idRolAdmin = await _usuarioRepository.GetIdRolAsync(RolAdministrador);
            if (idRolAdmin == null || await _usuarioRepository.ExisteUsuarioConRolAsync(idRolAdmin.Value))
                return false;

            await _usuarioRepository.AgregarAsync(new Usuario
            {
                IdPersonaNavigation = new Persona { Nombre = "Admin", Apellido = "Sistema" },
                IdRol = idRolAdmin,
                Email = email.Trim().ToLower(),
                PasswordHash = _passwordHasher.HashPassword(null!, password),
                IdEstadoUsuario = EstadoActivo,
                FechaRegistro = DateTime.Now
            });
            return true;
        }

        private bool PasswordValida(Usuario usuario, string password)
        {
            if (string.IsNullOrEmpty(usuario.PasswordHash)) return false;
            try
            {
                return _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password)
                       != PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                return false; // hash con formato inválido (ej. datos de prueba "HASH_FAKE_123")
            }
        }
    }
}
