namespace Domain
{
    public class Milk : BaseEntity
    {
        public virtual int Liters { get; set; }
        public virtual DateTime RecolectionDate { get; set; }

    }
}
