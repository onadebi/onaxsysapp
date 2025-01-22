namespace AppGlobal.Models
{
    public class CommonEntity
    {
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual DateTime? UpdatedAt { get; set; } = null;

        public virtual Guid Guid { get; set; } = Guid.NewGuid();
        public virtual bool IsActive { get; set; } = true;

        public virtual bool IsDeleted { get; set; } = false;
    }
}
