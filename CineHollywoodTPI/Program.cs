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

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<SistemaCineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IFuncionRepository, FuncionRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();
builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();

builder.Services.AddScoped<IPeliculaService, PeliculaService>();
builder.Services.AddScoped<IFuncionService, FuncionService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ISalaService, SalaService>();
builder.Services.AddScoped<IPromocionService, PromocionService>();
builder.Services.AddScoped<IPrecioService, PrecioService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.Configure<ReglasNegocioOptions>(builder.Configuration.GetSection(ReglasNegocioOptions.Seccion));

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddSingleton<PosterService>();
builder.Services.AddHttpClient("posters", http =>
{
    http.Timeout = TimeSpan.FromSeconds(15);
    http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) CineHollywoodTPI/1.0");
    http.DefaultRequestHeaders.Accept.ParseAdd("image/*,text/html;q=0.8,*/*;q=0.5");
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseCors("PermitirTodo");

app.UseMiddleware<ManejoErroresMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
