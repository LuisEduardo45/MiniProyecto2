using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace MvcTemplate.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Console.WriteLine("🔗 Conexión utilizada: " + this.Database.GetDbConnection().ConnectionString);
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<Gasto> Gastos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configura la relación entre Categoria y Usuario sin cascada
            builder.Entity<Categoria>()
                .HasOne<ApplicationUser>(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);  // <-- Aquí el cambio importante

            // Configuraciones existentes
            builder.Entity<Entrada>()
                .Property(e => e.Valor)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Gasto>()
                .Property(g => g.Monto)
                .HasColumnType("decimal(18,2)");
        }

    }
}
