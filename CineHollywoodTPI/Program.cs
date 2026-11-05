using System.Security.Claims;
using System.Text;
using CineHollywoodTPI.API.Middleware;
using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.API.Services.Interfaces;
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
// 3. INYECCIÓN DE DEPENDENCIAS
//    Controller  ->  Service (reglas de negocio)  ->  Repository (acceso a datos)
// ============================================================================

// 3.a Repositorios (capa de datos)
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IFuncionRepository, FuncionRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();

// 3.b Servicios (capa de negocio)
builder.Services.AddScoped<IPeliculaService, PeliculaService>();
builder.Services.AddScoped<IFuncionService, FuncionService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ISalaService, SalaService>();
builder.Services.AddScoped<IPrecioService, PrecioService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Parámetros de las reglas de negocio (descuentos, edad, topes...) desde appsettings.json
builder.Services.Configure<ReglasNegocioOptions>(builder.Configuration.GetSection(ReglasNegocioOptions.Seccion));

// 3.c Servicios de infraestructura
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>(); // hash seguro de contraseñas (PBKDF2)
builder.Services.AddSingleton<PosterService>();
builder.Services.AddHttpClient("posters", http =>
{
    http.Timeout = TimeSpan.FromSeconds(15);
    // Algunos sitios rechazan pedidos sin un User-Agent de navegador
    http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) CineHollywoodTPI/1.0");
    http.DefaultRequestHeaders.Accept.ParseAdd("image/*,text/html;q=0.8,*/*;q=0.5");
});

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
        var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var email = app.Configuration["AdminInicial:Email"] ?? "admin@cinehollywood.com";
        var password = app.Configuration["AdminInicial:Password"] ?? "Admin123!";

        if (await auth.CrearAdminInicialAsync(email, password))
            app.Logger.LogInformation("Usuario administrador creado: {Email}", email);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "No se pudo verificar/crear el usuario administrador inicial.");
    }
}

// ============================================================================
// 7. PIPELINE (el orden importa)
// ============================================================================
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

// CORS antes que todo lo demás, así las respuestas de error también llevan los headers CORS
app.UseCors("PermitirTodo");

// Convierte las excepciones de los servicios en 400 / 401 / 403 / 404 / 409 / 500
app.UseMiddleware<ManejoErroresMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
