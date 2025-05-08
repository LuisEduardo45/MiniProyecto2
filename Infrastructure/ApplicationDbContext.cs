using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        //adicionar aqui las entidades que se van a usar en la aplicacion
        public DbSet<Milk> Milks { get; set; }
        public IEnumerable<object> Categorias { get; set; }
        public object Gastos { get; set; }
    }
}