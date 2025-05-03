namespace Services.Dtos
{
    public class MilkModels
    {
        public List<MilkModel> Milks { get; set; }
        public MilkModels()
        {
            Milks = new List<MilkModel>();
        }
    }

    public class MilkModel
    {
        public Guid Id { get; set; }
        public int Liters { get; set; }
        public DateTime RecolectionDate { get; set; }

        public MilkModel()
        {
            RecolectionDate = DateTime.Now;
            Id = Guid.NewGuid();
        }
    }
}