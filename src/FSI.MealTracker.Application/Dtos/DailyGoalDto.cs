namespace FSI.MealTracker.Application.Dtos
{
    public class DailyGoalDto : BaseDto
    {
        public long UserId;
        public int TargetCalories;
        public DateTime GoalDate;
    }
}
