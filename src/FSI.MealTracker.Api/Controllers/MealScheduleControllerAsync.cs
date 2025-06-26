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
    [Route("api/mealschedule/async")]
    public class MealScheduleControllerAsync : BaseAsyncController<MealScheduleDto>
    {
        private readonly IMealScheduleAppService _service;

        public MealScheduleControllerAsync(IMealScheduleAppService service, ILogger<MealScheduleControllerAsync> logger,
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
                    _logger.LogWarning("MealSchedule with id {MealScheduleId} not found", id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving traffic with id {MealScheduleId}", id);
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MealScheduleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for traffic creation: {@MealScheduleDto}", dto);
                    return BadRequest(ModelState);
                }

                await _service.AddAsync(dto);

                _logger.LogInformation("MealSchedule created with id {MealScheduleId}", dto.Id);

                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {@MealScheduleDto}", dto);
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] MealScheduleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user update: {@MealScheduleDto}", dto);
                    return BadRequest(ModelState);
                }

                if (id != dto.Id)
                {
                    _logger.LogWarning("MealSchedule ID mismatch: route id = {RouteId}, dto id = {DtoId}", id, dto.Id);
                    return BadRequest("ID mismatch");
                }

                var existingMealSchedule = await _service.GetByIdAsync(id);
                if (existingMealSchedule is null)
                {
                    _logger.LogWarning("MealSchedule with id {MealScheduleId} not found for update", id);
                    return NotFound();
                }

                await _service.UpdateAsync(dto);

                _logger.LogInformation("MealSchedule with id {MealScheduleId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with id {MealScheduleId}", id);
                return StatusCode(500, "Error processing request");
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var existingMealSchedule = await _service.GetByIdAsync(id);
                if (existingMealSchedule is null)
                {
                    _logger.LogWarning("MealSchedule with id {MealScheduleId} not found for deletion", id);
                    return NotFound();
                }

                await _service.DeleteAsync(existingMealSchedule.Id);

                _logger.LogInformation("MealSchedule with id {MealScheduleId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id {MealScheduleId}", id);
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
            return await SendMessageAsync("getall", new MealScheduleDto(), "POST - MessageGetAll", "meal-schedule-queue");
        }

        [HttpPost("event/getbyid/{id:long}")]
        public async Task<IActionResult> MessageGetByIdAsync(long id)
        {
            return await SendMessageAsync("getbyid", new MealScheduleDto { Id = id }, "POST - MessageGetById", "meal-schedule-queue");
        }

        [HttpPost("event/create")]
        public async Task<IActionResult> MessageCreateAsync([FromBody] MealScheduleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await SendMessageAsync("create", dto, "POST - MessageCreate", "meal-schedule-queue");
        }

        [HttpPut("event/update/{id:long}")]
        public async Task<IActionResult> MessageUpdateAsync(long id, [FromBody] MealScheduleDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id)
                return BadRequest("Invalid payload or ID mismatch.");

            var existing = await _service.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            return await SendMessageAsync("update", dto, "PUT - MessageUpdate", "meal-schedule-queue");
        }

        [HttpGet("event/result/{id:long}")]
        public async Task<IActionResult> GetResultAsync(long id)
        {
            return await GetResultAsyncInternal(id, (action, messageResponse) =>
            {
                return action.ToLowerInvariant() switch
                {
                    "getall" => JsonSerializer.Deserialize<IEnumerable<MealScheduleDto>>(messageResponse),
                    "getbyid" => JsonSerializer.Deserialize<MealScheduleDto>(messageResponse),
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

            return await SendMessageAsync("delete", new MealScheduleDto { Id = id }, "DELETE - MessageDelete", "meal-schedule-queue");
        }

        #endregion

        #region Additional Methods  

        #endregion

    }
}