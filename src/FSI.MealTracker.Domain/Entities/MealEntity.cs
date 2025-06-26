namespace FSI.MealTracker.Domain.Entities
{
    public class MealEntity : BaseEntity
    {
        public long UserId { get; set; }
        public UserEntity? User { get; set; }

        public ICollection<ConsumptionEntity> Consumptions { get; set; } = new List<ConsumptionEntity>();
        public ICollection<MealScheduleEntity> Schedules { get; set; } = new List<MealScheduleEntity>();
    }
}
