namespace FSI.MealTracker.Domain.Entities
{
    public class FoodEntity : BaseEntity
    {
        public int CaloriesPerPortion { get; set; }
        public string StandardPortion { get; set; } = string.Empty;

        public ICollection<ConsumptionEntity> Consumptions { get; set; } = new List<ConsumptionEntity>();
    }
}
