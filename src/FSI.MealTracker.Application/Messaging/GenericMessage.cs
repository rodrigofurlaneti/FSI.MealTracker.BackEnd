namespace FSI.MealTracker.Application.Messaging
{
    public class GenericMessage<T> where T : class
    {
        public string Action { get; set; }
        public T Payload { get; set; }
        public long MessagingId { get; set; }
    }
}
