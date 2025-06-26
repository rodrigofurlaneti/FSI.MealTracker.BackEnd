using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Application.Interfaces;
using FSI.MealTracker.Application.Mapper;
using FSI.MealTracker.Domain.Interfaces;

namespace FSI.MealTracker.Application.Services
{
    public class MessagingAppService : IMessagingAppService
    {
        private readonly IMessagingRepository _repository; // ✅ Declaração

        public MessagingAppService(IMessagingRepository repository) // ✅ Construtor
        {
            _repository = repository;
        }

        #region Methods from IBaseAppService

        public async Task<IEnumerable<MessagingDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MessagingMapper.ToDto);
        }


        public async Task<MessagingDto?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity is null ? null : MessagingMapper.ToDto(entity);
        }

        public async Task<long> AddAsync(MessagingDto dto)
        {
            var entity = MessagingMapper.ToEntity(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(MessagingDto dto)
        {
            var entity = MessagingMapper.ToEntity(dto);
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = new Domain.Entities.MessagingEntity { Id = id };
            return await _repository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<MessagingDto>> GetAllFilteredAsync(string filterBy, string value)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<MessagingDto>> GetAllOrderedAsync(string orderBy, string direction)
        {
            throw new NotImplementedException();
        }

         #endregion
    }
}