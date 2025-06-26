namespace FSI.MealTracker.Domain.Entities
{
    public class DailyGoalEntity : BaseEntity
    {
        public long UserId { get; set; }
        public UserEntity? User { get; set; }

        public int TargetCalories { get; set; }
        public DateTime GoalDate { get; set; }
    }
}
