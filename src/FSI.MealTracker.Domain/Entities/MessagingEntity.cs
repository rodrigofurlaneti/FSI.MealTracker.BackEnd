namespace FSI.MealTracker.Domain.Entities
{
    public class MessagingEntity : BaseEntity
    {
        public string ActionMessaging { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string MessageRequest { get; set; } = string.Empty;
        public string MessageResponse { get; set; } = string.Empty;
        public bool IsProcessed { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public MessagingEntity() { }

        public MessagingEntity(string actionMessaging, string queueName, string messageRequest, bool isProcessed, string errorMessage)
        {
            ActionMessaging = actionMessaging;
            QueueName = queueName;
            MessageRequest = messageRequest;
            IsProcessed = isProcessed;
            ErrorMessage = errorMessage;
        }

        public MessagingEntity(string actionMessaging, string queueName, string messageRequest, string messageResponse, bool isProcessed, string errorMessage)
        {
            ActionMessaging = actionMessaging;
            QueueName = queueName;
            MessageRequest = messageRequest;
            MessageResponse = messageResponse;
            IsProcessed = isProcessed;
            ErrorMessage = errorMessage;
        }
    }
}