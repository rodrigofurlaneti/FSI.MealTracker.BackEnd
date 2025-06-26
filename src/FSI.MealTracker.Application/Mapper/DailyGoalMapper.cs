using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public class DailyGoalMapper : BaseMapper<DailyGoalDto, DailyGoalEntity>
    {
        public override DailyGoalDto ToDto(DailyGoalEntity entity) => new DailyGoalDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            UserId = entity.UserId,
            TargetCalories = entity.TargetCalories,
            GoalDate = entity.GoalDate,
        };

        public override DailyGoalEntity ToEntity(DailyGoalDto dto) => new DailyGoalEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            UserId = dto.UserId,
            TargetCalories = dto.TargetCalories,
            GoalDate = dto.GoalDate,
        };
    }
}