namespace FSI.MealTracker.Application.Dtos
{
    public class MessagingDto
    {
        public long Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string MessageRequest { get; set; } = string.Empty;
        public string MessageResponse { get; set; } = string.Empty;
        public bool IsProcessed { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public MessagingDto()
        {

        }

        public MessagingDto(long id)
        {
            Id = id;
        }

        public MessagingDto(string action, string queueName, string messageRequest, bool isProcessed, string errorMessage)
        {
            Action = action;
            QueueName = queueName;
            MessageRequest = messageRequest;
            IsProcessed = isProcessed;
            ErrorMessage = errorMessage;
        }

        public MessagingDto(string action, string queueName, string messageRequest, string messageResponse, bool isProcessed, string errorMessage)
        {
            Action = action;
            QueueName = queueName;
            MessageRequest = messageRequest;
            MessageResponse = messageResponse;
            IsProcessed = isProcessed;
            ErrorMessage = errorMessage;
        }
    }
}