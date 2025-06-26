using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public class MealScheduleMapper : BaseMapper<MealScheduleDto, MealScheduleEntity>
    {
        public override MealScheduleDto ToDto(MealScheduleEntity entity) => new MealScheduleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            UserId = entity.UserId,
            MealId = entity.MealId,
            ScheduledDate = entity.ScheduledDate,
            Notes = entity.Notes,
        };

        public override MealScheduleEntity ToEntity(MealScheduleDto dto) => new MealScheduleEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            UserId = dto.UserId,
            MealId = dto.MealId,
            ScheduledDate = dto.ScheduledDate,
            Notes = dto.Notes,
        };
    }
}