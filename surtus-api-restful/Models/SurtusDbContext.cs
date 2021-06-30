using System;
using Microsoft.EntityFrameworkCore;

namespace surtus_api_restful.Models
{
    public class SurtusDbContext : DbContext
    {
        public SurtusDbContext(DbContextOptions<SurtusDbContext> options) : base(options) { }

        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Clase> Clases { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Reto> Retos { get; set; }
        public DbSet<EvaluacionReto> EvaluacionRetos { get; set; }
        public DbSet<InscritoModulo> InscritoModulos { get; set; }
        public DbSet<InscritoClase> InscritoClases { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Inscripcion>(entity =>
            {
                entity.ToTable(nameof(Inscripcion));

                entity.ConfigurarIdentityLongId();

                entity.Property(e => e.Nombres).IsRequiredVariableLengthString(50, false);
                entity.Property(e => e.Apellidos).IsRequiredVariableLengthString(50, false);
                entity.Property(e => e.Usuario).IsRequiredVariableLengthString(20, false);
                entity.Property(e => e.Clave).IsRequiredVariableLengthString(100, false);
                entity.Property(e => e.Nivel).IsRequiredVariableLengthString(15, false);

                entity.HasKey(e => e.Id);
            });

            builder.Entity<Modulo>(entity =>
            {
                entity.ToTable(nameof(Modulo));

                entity.ConfigurarIdentityLongId();

                entity.Property(e => e.Nombre).IsRequiredVariableLengthString(50, false);
                entity.Property(e => e.Imagen).IsRequiredVariableLengthString(200, false);
                entity.Property(e => e.Nivel).IsRequiredVariableLengthString(15, false);

                entity.HasKey(e => e.Id);
            });

            builder.Entity<Clase>(entity =>
            {
                entity.ToTable(nameof(Clase));

                entity.ConfigurarIdentityLongId();

                entity.Property(e => e.Nombre).IsRequiredVariableLengthString(50, false);
                entity.Property(e => e.Imagen).IsRequiredVariableLengthString(200, false);

                entity.MapearUnoMuchosUnidireccional(e => e.Modulo, e => e.IdModulo);

                entity.HasKey(e => e.Id);
            });

            builder.Entity<Reto>(entity =>
            {
                entity.ToTable(nameof(Reto));

                entity.ConfigurarIdentityLongId();

                entity.Property(e => e.Nombre).IsRequiredVariableLengthString(50, false);
                entity.Property(e => e.Estado).IsRequiredVariableLengthString(1, false);

                entity.MapearUnoMuchosUnidireccional(e => e.Modulo, e => e.IdModulo);
                entity.MapearUnoMuchosUnidireccional(e => e.Inscripcion, e => e.IdInscrito);

                entity.HasKey(e => e.Id);
            });

            builder.Entity<EvaluacionReto>(entity =>
            {
                entity.ToTable(nameof(EvaluacionReto));

                entity.HasKey(e => e.IdReto);

                entity.Property(e => e.Estado).IsRequiredVariableLengthString(20, false);

                entity.MapearUnoMuchosBidireccional(e => e.Reto, e => e.EvaluacionRetos, e => e.IdReto);
            });

            builder.Entity<InscritoModulo>(entity =>
            {
                entity.ToTable(nameof(InscritoModulo));

                entity.HasKey(e => new { e.IdInscrito, e.IdModulo });

                entity.Property(e => e.Avance).HasPrecision(5, 2);

                entity.MapearUnoMuchosBidireccional(e => e.Inscripcion, e => e.InscritoModulos, e => e.IdInscrito);
                entity.MapearUnoMuchosBidireccional(e => e.Modulo, e => e.InscritoModulos, e => e.IdModulo);
            });

            builder.Entity<InscritoClase>(entity =>
            {
                entity.ToTable(nameof(InscritoClase));

                entity.HasKey(e => new { e.IdInscrito, e.IdClase });

                entity.MapearUnoMuchosBidireccional(e => e.Inscripcion, e => e.InscritoClases, e => e.IdInscrito);
                entity.MapearUnoMuchosBidireccional(e => e.Clase, e => e.InscritoClases, e => e.IdClase);

            });
        }
    }
}
