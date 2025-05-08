using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniProyecto2.Web.Models;
using MvcTemplate.Models;

namespace MiniProyecto2.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Entrada> Entradas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Puedes agregar configuraciones adicionales aquí si lo deseas
        }
    }
}
