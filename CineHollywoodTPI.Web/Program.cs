var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Sirve index.html al entrar a "/" y todos los archivos de wwwroot (HTML, CSS, JS, imágenes).
// Toda la lógica está en el frontend (JS) que consume la API en http://localhost:5191/api
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
