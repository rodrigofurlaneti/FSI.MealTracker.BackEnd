using Dapper;
using FSI.MealTracker.Domain.Entities;
using FSI.MealTracker.Domain.Interfaces;
using FSI.MealTracker.Infrastructure.Context;
using System.Data;

namespace FSI.MealTracker.Infrastructure.Repositories
{
    public class MealScheduleRepository : BaseRepository, IMealScheduleRepository
    {
        private const string PROCEDURE_NAME = "usp_MealSchedule";
        private const string ACTION_GETALL = "GetAll";
        private const string ACTION_GETBYID = "GetById";
        private const string ACTION_INSERT = "Insert";
        private const string ACTION_UPDATE = "Update";
        private const string ACTION_DELETE = "Delete";

        public MealScheduleRepository(IDbContext context) : base(context) { }

        public async Task<IEnumerable<MealScheduleEntity>> GetAllAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<MealScheduleEntity>(
                PROCEDURE_NAME,
                new { Action = ACTION_GETALL },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<MealScheduleEntity?> GetByIdAsync(long id)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<MealScheduleEntity>(
                PROCEDURE_NAME,
                new { Action = ACTION_GETBYID, Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<long> AddAsync(MealScheduleEntity entity)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<long>(
                PROCEDURE_NAME,
                new
                {
                    Action = ACTION_INSERT,
                    entity.Name,
                    entity.IsActive,
                    entity.CreatedAt,
                    entity.UpdatedAt
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> UpdateAsync(MealScheduleEntity entity)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<bool>(
                PROCEDURE_NAME,
                new
                {
                    Action = ACTION_UPDATE,
                    entity.Id,
                    entity.Name,
                    entity.IsActive,
                    entity.UpdatedAt
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteAsync(MealScheduleEntity entity)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<bool>(
                PROCEDURE_NAME,
                new { Action = ACTION_DELETE, entity.Id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<MealScheduleEntity>> GetAllFilteredAsync(string filterBy, string value)
        {
            using var connection = CreateConnection();
            if (!_orderMap.ContainsKey(filterBy))
                throw new ArgumentException("Invalid filterBy field");

            var procedureName = $"usp_MealSchedule_GetAll_FilterBy_{_orderMap[filterBy]}";
            var parameters = new DynamicParameters();
            parameters.Add(_orderMap[filterBy], value);

            return await connection.QueryAsync<MealScheduleEntity>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MealScheduleEntity>> GetAllOrderedAsync(string orderBy, string direction)
        {
            using var connection = CreateConnection();
            if (!_orderMap.ContainsKey(orderBy))
                throw new ArgumentException("Invalid orderBy field");

            var isDesc = direction.Equals("desc", StringComparison.OrdinalIgnoreCase);
            var procedureName = $"usp_MealSchedule_GetAll_OrderBy_{_orderMap[orderBy]}_{(isDesc ? "Desc" : "Asc")}";

            return await connection.QueryAsync<MealScheduleEntity>(procedureName, commandType: CommandType.StoredProcedure);
        }

        private static readonly Dictionary<string, string> _orderMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", "Name" }
        };
    }
}
