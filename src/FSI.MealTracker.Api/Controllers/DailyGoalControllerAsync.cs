using FSI.MealTracker.Api.Controllers.Base;
using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Application.Interfaces;
using FSI.MealTracker.Api.Controllers.Base;
using FSI.MealTracker.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FSI.MealTracker.Api.Controllers
{
    [ApiController]
    [Route("api/dailygoals/async")]
    public class DailyGoalControllerAsync : BaseAsyncController<DailyGoalDto>
    {
        private readonly IDailyGoalAppService _service;

        public DailyGoalControllerAsync(IDailyGoalAppService service, ILogger<DailyGoalControllerAsync> logger,
            IMessageQueuePublisher publisher, IMessagingAppService messagingService) : base(logger, publisher, messagingService)
        {
            _service = service;
        }

        #region CRUD Operations

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error getting user");
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (result is null)
                {
                    _logger.LogWarning("DailyGoal with id {DailyGoalId} not found", id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving traffic with id {DailyGoalId}", id);
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DailyGoalDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for traffic creation: {@DailyGoalDto}", dto);
                    return BadRequest(ModelState);
                }

                await _service.AddAsync(dto);

                _logger.LogInformation("DailyGoal created with id {DailyGoalId}", dto.Id);

                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {@DailyGoalDto}", dto);
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] DailyGoalDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user update: {@DailyGoalDto}", dto);
                    return BadRequest(ModelState);
                }

                if (id != dto.Id)
                {
                    _logger.LogWarning("DailyGoal ID mismatch: route id = {RouteId}, dto id = {DtoId}", id, dto.Id);
                    return BadRequest("ID mismatch");
                }

                var existingDailyGoal = await _service.GetByIdAsync(id);
                if (existingDailyGoal is null)
                {
                    _logger.LogWarning("DailyGoal with id {DailyGoalId} not found for update", id);
                    return NotFound();
                }

                await _service.UpdateAsync(dto);

                _logger.LogInformation("DailyGoal with id {DailyGoalId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with id {DailyGoalId}", id);
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var existingDailyGoal = await _service.GetByIdAsync(id);
                if (existingDailyGoal is null)
                {
                    _logger.LogWarning("DailyGoal with id {DailyGoalId} not found for deletion", id);
                    return NotFound();
                }

                await _service.DeleteAsync(existingDailyGoal.Id);

                _logger.LogInformation("DailyGoal with id {DailyGoalId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id {DailyGoalId}", id);
                return StatusCode(500, "Error processing request");
            }
        }

        //[HttpGet("filtered")]
        //public async Task<IActionResult> GetAllFiltered([FromQuery] string filterBy, [FromQuery] string value)
        //{
        //    try
        //    {
        //        var result = await _service.GetAllFilteredAsync(filterBy, value);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error filtering user by {FilterBy} ", filterBy);
        //        return StatusCode(500, "Error processing request");
        //    }
        //}

        //[HttpGet("ordered")]
        //public async Task<IActionResult> GetAllOrdered([FromQuery] string orderBy, [FromQuery] string direction = "asc")
        //{
        //    try
        //    {
        //        var result = await _service.GetAllOrderedAsync(orderBy, direction);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error ordering user by {OrderBy} {Direction}", orderBy, direction);
        //        return StatusCode(500, "Error processing request");
        //    }
        //}

        #endregion

        #region CRUD Operations Async - Event Driven Architecture - Request response via polling - Async Message Dispatch with Deferred Response

        [HttpPost("event/getall")]
        public async Task<IActionResult> MessageGetAllAsync()
        {
            return await SendMessageAsync("getall", new DailyGoalDto(), "POST - MessageGetAll", "user-queue");
        }

        [HttpPost("event/getbyid/{id:long}")]
        public async Task<IActionResult> MessageGetByIdAsync(long id)
        {
            return await SendMessageAsync("getbyid", new DailyGoalDto { Id = id }, "POST - MessageGetById", "user-queue");
        }

        [HttpPost("event/create")]
        public async Task<IActionResult> MessageCreateAsync([FromBody] DailyGoalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await SendMessageAsync("create", dto, "POST - MessageCreate", "user-queue");
        }

        [HttpPut("event/update/{id:long}")]
        public async Task<IActionResult> MessageUpdateAsync(long id, [FromBody] DailyGoalDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id)
                return BadRequest("Invalid payload or ID mismatch.");

            var existing = await _service.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            return await SendMessageAsync("update", dto, "PUT - MessageUpdate", "user-queue");
        }

        [HttpGet("event/result/{id:long}")]
        public async Task<IActionResult> GetResultAsync(long id)
        {
            return await GetResultAsyncInternal(id, (action, messageResponse) =>
            {
                return action.ToLowerInvariant() switch
                {
                    "getall" => JsonSerializer.Deserialize<IEnumerable<DailyGoalDto>>(messageResponse),
                    "getbyid" => JsonSerializer.Deserialize<DailyGoalDto>(messageResponse),
                    "create" or "update" or "delete" => messageResponse,
                    _ => null
                };
            });
        }

        [HttpDelete("event/delete/{id:long}")]
        public async Task<IActionResult> MessageDeleteAsync(long id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            return await SendMessageAsync("delete", new DailyGoalDto { Id = id }, "DELETE - MessageDelete", "user-queue");
        }

        #endregion

        #region Additional Methods  

        #endregion

    }
}