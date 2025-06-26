namespace FSI.MealTracker.Domain.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        protected BaseEntity()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsActive = true;
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        public void UpdateTimestamp() => UpdatedAt = DateTime.Now;
    }
}