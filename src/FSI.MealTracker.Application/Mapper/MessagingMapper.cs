using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public static class MessagingMapper
    {
        public static MessagingEntity ToEntity(MessagingDto dto)
        {
            return new MessagingEntity
            {
                Id = dto.Id,
                ActionMessaging = dto.ActionMessaging,
                QueueName = dto.QueueName,
                MessageRequest = dto.MessageRequest,
                MessageResponse = dto.MessageResponse,
                IsProcessed = dto.IsProcessed,
                ErrorMessage = dto.ErrorMessage,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = dto.IsActive
            };
        }

        public static MessagingDto ToDto(MessagingEntity entity)
        {
            return new MessagingDto
            {
                Id = entity.Id,
                ActionMessaging = entity.ActionMessaging,
                QueueName = entity.QueueName,
                MessageRequest = entity.MessageRequest,
                MessageResponse = entity.MessageResponse,
                IsProcessed = entity.IsProcessed,
                ErrorMessage = entity.ErrorMessage,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = DateTime.Now,
                IsActive = entity.IsActive
            };
        }
    }
}