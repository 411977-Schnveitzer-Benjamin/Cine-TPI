using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Models;

// Configuración extra que NO está en el scaffold (así no se pierde si se vuelve a generar el contexto).
public partial class SistemaCineContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // La columna PosterUrl existe en la tabla PELICULA pero no estaba mapeada.
        modelBuilder.Entity<Pelicula>()
            .Property(p => p.PosterUrl)
            .HasMaxLength(300)
            .IsUnicode(false)
            .HasColumnName("PosterUrl");
    }
}
