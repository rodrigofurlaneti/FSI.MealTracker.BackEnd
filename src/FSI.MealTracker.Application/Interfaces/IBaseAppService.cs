namespace FSI.MealTracker.Application.Interfaces
{
    public interface IBaseAppService<TDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(long id);
        Task<long> AddAsync(TDto dto);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
