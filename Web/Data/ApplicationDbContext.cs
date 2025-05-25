using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Models;
using MvcTemplate.Data;
using System.Reflection.Emit;

namespace MvcTemplate.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
            builder.Entity<Entrada>()
    .Property(e => e.Valor)
    .HasColumnType("decimal(18,2)");

            builder.Entity<Gasto>()
                .Property(g => g.Monto)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Categoria>()
    .Property(c => c.TopeMaximo)
    .HasColumnType("decimal(18,2)");

            // Puedes agregar configuraciones adicionales aquí si lo deseas
        }
    }
}
