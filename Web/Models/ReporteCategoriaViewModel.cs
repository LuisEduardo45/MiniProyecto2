namespace MvcTemplate.Models.ViewModels
{
    public class ReporteCategoriaViewModel
    {
        public string? Titulo { get; set; }
        public int PorcentajeMaximo { get; set; }
        public decimal GastoTotal { get; set; }
        public decimal TopePermitido { get; set; }
        public bool Excedido => GastoTotal > TopePermitido;
    }
}
