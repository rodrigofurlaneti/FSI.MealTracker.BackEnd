namespace FSI.MealTracker.Domain.Entities
{
    public class MealScheduleEntity : BaseEntity
    {
        public long UserId { get; set; }
        public UserEntity? User { get; set; }

        public long MealId { get; set; }
        public MealEntity? Meal { get; set; }

        public DateTime ScheduledDate { get; set; }
        public string? Notes { get; set; }
    }
}