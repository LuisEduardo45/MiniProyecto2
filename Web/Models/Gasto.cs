namespace MvcTemplate.Models
{
    public class Gasto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }

        public int CategoriaId { get; set; }  // Nueva propiedad para ligar gasto con categoría
        public Categoria Categoria { get; set; }
    }
}
