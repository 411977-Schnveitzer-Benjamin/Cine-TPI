using Microsoft.EntityFrameworkCore;
using CineHollywoodTPI.Data.Models;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. CONFIGURACIÓN DE POLÍTICAS CORS (Para permitir que tu Web consuma la API)
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()   // Permite peticiones desde cualquier origen (ej. tu proyecto Web)
              .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();  // Permite cualquier tipo de cabecera
    });
});

// ============================================================================
// 2. CONEXIÓN A LA BASE DE DATOS (Usando tu DefaultConnection)
// ============================================================================
builder.Services.AddDbContext<SistemaCineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================================
// 3. REGISTRO DE REPOSITORIOS (Inyección de Dependencias)
// ============================================================================
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IFuncionRepository, FuncionRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

// ============================================================================
// 4. SOPORTE PARA CONTROLADORES Y SWAGGER (Para probar tus endpoints)
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del entorno de desarrollo (Swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ¡CLAVE! Activar la política CORS antes de mapear rutas o autorizaciones
app.UseCors("PermitirTodo");

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapea los controladores tradicionales de la carpeta Controllers
app.MapControllers();

app.Run();
