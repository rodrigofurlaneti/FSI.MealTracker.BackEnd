using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Application.Mapper
{
    public class UserMapper : BaseMapper<UserDto, UserEntity>
    {
        public override UserDto ToDto(UserEntity entity) => new UserDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        public override UserEntity ToEntity(UserDto dto) => new UserEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
