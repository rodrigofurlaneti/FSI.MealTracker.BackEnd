using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public class MealMapper : BaseMapper<MealDto, MealEntity>
    {
        public override MealDto ToDto(MealEntity entity) => new MealDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            UserId = entity.UserId,
        };

        public override MealEntity ToEntity(MealDto dto) => new MealEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            UserId = dto.UserId,
        };
    }
}