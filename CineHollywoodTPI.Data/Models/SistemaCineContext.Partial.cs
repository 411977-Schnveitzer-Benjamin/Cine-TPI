using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Models;

public partial class SistemaCineContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pelicula>()
            .Property(p => p.PosterUrl)
            .HasMaxLength(300)
            .IsUnicode(false)
            .HasColumnName("PosterUrl");
    }
}
