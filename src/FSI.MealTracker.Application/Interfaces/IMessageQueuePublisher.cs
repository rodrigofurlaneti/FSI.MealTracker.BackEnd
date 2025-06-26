namespace FSI.MealTracker.Application.Interfaces
{
    public interface IMessageQueuePublisher
    {
        void Publish<T>(T message, string queueName);
    }
}
