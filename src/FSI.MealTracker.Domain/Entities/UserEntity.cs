namespace FSI.MealTracker.Domain.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navegação
        public ICollection<MealEntity> Meals { get; set; } = new List<MealEntity>();
        public ICollection<ConsumptionEntity> Consumptions { get; set; } = new List<ConsumptionEntity>();
        public ICollection<DailyGoalEntity> DailyGoals { get; set; } = new List<DailyGoalEntity>();
        public ICollection<MealScheduleEntity> MealSchedules { get; set; } = new List<MealScheduleEntity>();
    }
}
