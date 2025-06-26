using Dapper;
using FSI.MealTracker.Domain.Entities;
using FSI.MealTracker.Domain.Interfaces;
using FSI.MealTracker.Infrastructure.Context;
using System.Data;

namespace FSI.MealTracker.Infrastructure.Repositories
{
    public class MessagingRepository : BaseRepository, IMessagingRepository
    {
        private const string PROCEDURE_NAME = "usp_Messaging";
        private const string ACTION_GETALL = "GetAll";
        private const string ACTION_GETBYID = "GetById";
        private const string ACTION_INSERT = "Insert";
        private const string ACTION_UPDATE = "Update";
        private const string ACTION_DELETE = "Delete";

        public MessagingRepository(IDbContext context) : base(context) { }

        public async Task<IEnumerable<MessagingEntity>> GetAllAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<MessagingEntity>(
                PROCEDURE_NAME,
                new { Action = ACTION_GETALL },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<MessagingEntity?> GetByIdAsync(long id)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<MessagingEntity>(
                PROCEDURE_NAME,
                new { Action = ACTION_GETBYID, Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<long> AddAsync(MessagingEntity entity)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<long>(
                PROCEDURE_NAME,
                new
                {
                    Action = ACTION_INSERT,
                    OperationMessage = entity.OperationMessage,
                    entity.QueueName,
                    entity.MessageRequest,
                    entity.MessageResponse,
                    entity.ErrorMessage,
                    entity.CreatedAt,
                    entity.UpdatedAt,
                    entity.IsActive
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> UpdateAsync(MessagingEntity entity)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<bool>(
                PROCEDURE_NAME,
                new
                {
                    Action = ACTION_UPDATE,
                    OperationMessage = entity.OperationMessage,
                    entity.Id,
                    entity.QueueName,
                    entity.MessageRequest,
                    entity.MessageResponse,
                    entity.IsProcessed,
                    entity.ErrorMessage,
                    entity.CreatedAt,
                    entity.UpdatedAt,
                    entity.IsActive
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteAsync(MessagingEntity entity)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<bool>(
                PROCEDURE_NAME,
                new { OperationMessage = ACTION_DELETE, entity.Id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<MessagingEntity>> GetAllFilteredAsync(string filterBy, string value)
        {
            using var connection = CreateConnection();
            if (!_orderMap.ContainsKey(filterBy))
                throw new ArgumentException("Invalid filterBy field");

            var procedureName = $"usp_Messaging_GetAll_FilterBy_{_orderMap[filterBy]}";
            var parameters = new DynamicParameters();
            parameters.Add(_orderMap[filterBy], value);

            return await connection.QueryAsync<MessagingEntity>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MessagingEntity>> GetAllOrderedAsync(string orderBy, string direction)
        {
            using var connection = CreateConnection();
            if (!_orderMap.ContainsKey(orderBy))
                throw new ArgumentException("Invalid orderBy field");

            var isDesc = direction.Equals("desc", StringComparison.OrdinalIgnoreCase);
            var procedureName = $"usp_Messaging_GetAll_OrderBy_{_orderMap[orderBy]}_{(isDesc ? "Desc" : "Asc")}";

            return await connection.QueryAsync<MessagingEntity>(procedureName, commandType: CommandType.StoredProcedure);
        }

        private static readonly Dictionary<string, string> _orderMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "Id" }
        };
    }
}
