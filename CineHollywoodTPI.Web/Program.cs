var builder = WebApplication.CreateBuilder(args);

// Habilitar servicios para páginas web estáticas si usan MVC o Razor Pages básicas
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ¡CLAVE! Esto le permite al proyecto leer la carpeta wwwroot (donde Matías meterá el HTML/CSS/JS)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
