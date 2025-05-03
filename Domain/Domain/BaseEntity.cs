using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public abstract class BaseEntity
    {
        [Key]
        public virtual Guid Id { get; set; }

    }
}
