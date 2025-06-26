using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public class ConsumptionMapper : BaseMapper<ConsumptionDto, ConsumptionEntity>
    {
        public override ConsumptionDto ToDto(ConsumptionEntity entity) => new ConsumptionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            UserId = entity.UserId,
            MealId = entity.MealId,
            FoodId = entity.FoodId,
            ConsumptionDate = entity.ConsumptionDate,
            Quantity = entity.Quantity,
        };

        public override ConsumptionEntity ToEntity(ConsumptionDto dto) => new ConsumptionEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            UserId = dto.UserId,
            MealId = dto.MealId,
            FoodId = dto.FoodId,
            ConsumptionDate = dto.ConsumptionDate,
            Quantity = dto.Quantity,
        };
    }
}