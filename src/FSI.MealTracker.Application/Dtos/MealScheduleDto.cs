namespace FSI.MealTracker.Application.Dtos
{
    public class MealScheduleDto : BaseDto
    {
        public long UserId;
        public long MealId;
        public DateTime ScheduledDate;
        public string? Notes;
    }
}
