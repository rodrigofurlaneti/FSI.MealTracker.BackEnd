using FSI.MealTracker.Domain.Entities;

namespace FSI.MealTracker.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        #region Entity default methods

        // Get All
        Task<IEnumerable<T>> GetAllAsync();

        // Get By Id
        Task<T?> GetByIdAsync(long id);

        // Add
        Task<long> AddAsync(T entity);

        // Update
        Task<bool> UpdateAsync(T entity);

        //Delete
        Task<bool> DeleteAsync(T entity);

        // Ordered
        Task<IEnumerable<T>> GetAllOrderedAsync(string orderBy, string direction);

        // Filtered
        Task<IEnumerable<T>> GetAllFilteredAsync(string filterBy, string value);

        #endregion
    }
}