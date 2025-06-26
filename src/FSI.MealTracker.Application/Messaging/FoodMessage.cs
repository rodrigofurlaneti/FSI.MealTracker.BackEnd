using FSI.MealTracker.Application.Dtos;

namespace FSI.MealTracker.Application.Messaging
{
    public class FoodMessage
    {
        public string Action { get; set; } = string.Empty; // "Create", "Update", "Delete"
        public FoodDto Payload { get; set; } = new();
        public long MessagingId { get; set; } // Id da mensagem na tabela Messaging
    }
}
