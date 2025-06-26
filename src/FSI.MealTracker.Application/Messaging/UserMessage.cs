using FSI.MealTracker.Application.Dtos;

namespace FSI.MealTracker.Application.Messaging
{
    public class UserMessage
    {
        public string Action { get; set; } = string.Empty; // "Create", "Update", "Delete"
        public UserDto Payload { get; set; } = new();
        public long MessagingId { get; set; } // Id da mensagem na tabela Messaging
    }
}
