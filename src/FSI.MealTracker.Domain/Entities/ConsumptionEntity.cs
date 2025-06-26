namespace FSI.MealTracker.Domain.Entities
{
    public class ConsumptionEntity : BaseEntity
    {
        public long UserId { get; set; }
        public UserEntity? User { get; set; }

        public long MealId { get; set; }
        public MealEntity? Meal { get; set; }

        public long FoodId { get; set; }
        public FoodEntity? Food { get; set; }

        public DateTime ConsumptionDate { get; set; }
        public int Quantity { get; set; }
    }
}