using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public class FoodMapper : BaseMapper<FoodDto, FoodEntity>
    {
        public override FoodDto ToDto(FoodEntity entity) => new FoodDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CaloriesPerPortion = entity.CaloriesPerPortion,
            StandardPortion = entity.StandardPortion,
        };

        public override FoodEntity ToEntity(FoodDto dto) => new FoodEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CaloriesPerPortion = dto.CaloriesPerPortion,
            StandardPortion = dto.StandardPortion,
        };
    }
}