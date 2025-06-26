using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Application.Interfaces;
using FSI.MealTracker.Application.Mapper;
using FSI.MealTracker.Domain.Entities;
using FSI.MealTracker.Domain.Interfaces;

namespace FSI.MealTracker.Application.Services
{
    public class DailyGoalAppService : BaseAppService<DailyGoalDto, DailyGoalEntity>, IDailyGoalAppService
    {
        public DailyGoalAppService(IDailyGoalRepository repository)
            : base(repository, new DailyGoalMapper()) { }
    }
}