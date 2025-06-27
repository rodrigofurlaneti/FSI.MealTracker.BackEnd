using System.Text.Json.Serialization;

namespace FSI.MealTracker.Application.Dtos
{
    public class FoodDto : BaseDto
    {
        [JsonPropertyName("caloriesPerPortion")]
        public int CaloriesPerPortion { get; set; }
        [JsonPropertyName("standardPortion")]
        public string StandardPortion { get; set; } = string.Empty;
    }
}
