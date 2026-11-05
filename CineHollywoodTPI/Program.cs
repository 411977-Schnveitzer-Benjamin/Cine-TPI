using System.Security.Claims;
using System.Text;
using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using CineHollywoodTPI.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. CORS (para que el proyecto Web pueda consumir la API desde otro puerto)
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================================================
// 2. BASE DE DATOS
// ============================================================================
builder.Services.AddDbContext<SistemaCineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================================
// 3. REPOSITORIOS Y SERVICIOS (Inyección de Dependencias)
// ============================================================================
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IFuncionRepository, FuncionRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();

builder.Services.AddScoped<TokenService>();

// Pósters: se guardan en wwwroot/posters. El HttpClient se usa para descargar imágenes desde un link.
builder.Services.AddSingleton<PosterService>();
builder.Services.AddHttpClient("posters", http =>
{
    http.Timeout = TimeSpan.FromSeconds(15);
    // Algunos sitios rechazan pedidos sin un User-Agent de navegador
    http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) CineHollywoodTPI/1.0");
    http.DefaultRequestHeaders.Accept.ParseAdd("image/*,text/html;q=0.8,*/*;q=0.5");
});
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>(); // Hash seguro de contraseñas (PBKDF2)

// ============================================================================
// 4. AUTENTICACIÓN JWT
// ============================================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// ============================================================================
// 5. CONTROLADORES Y SWAGGER (con botón "Authorize" para probar con el token)
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cine Hollywood API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá acá el token que devuelve POST /api/auth/login"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ============================================================================
// 6. USUARIO ADMINISTRADOR INICIAL (se crea solo si no existe ningún admin)
// ============================================================================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var usuarios = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
        var email = app.Configuration["AdminInicial:Email"] ?? "admin@cinehollywood.com";
        var password = app.Configuration["AdminInicial:Password"] ?? "Admin123!";

        if (await usuarios.CrearAdminSiNoExisteAsync(email, hasher.HashPassword(null!, password)))
            app.Logger.LogInformation("Usuario administrador creado: {Email}", email);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "No se pudo verificar/crear el usuario administrador inicial.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En desarrollo NO redirigimos a HTTPS: el frontend llama a http://localhost:5191
    // y la redirección rompe las peticiones CORS (preflight).
    app.UseHttpsRedirection();
}

// Sirve las imágenes guardadas: http://localhost:5191/posters/archivo.jpg
app.UseStaticFiles();

// CORS antes de autenticación/autorización
app.UseCors("PermitirTodo");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
