using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Application.Interfaces;
using FSI.MealTracker.Application.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FSI.MealTracker.Api.Controllers.Base
{
    public abstract class BaseAsyncController<TDto> : ControllerBase where TDto : class
    {
        protected readonly ILogger _logger;
        protected readonly IMessageQueuePublisher _publisher;
        protected readonly IMessagingAppService _messagingAppService;

        protected BaseAsyncController(
            ILogger logger,
            IMessageQueuePublisher publisher,
            IMessagingAppService messagingAppService)
        {
            _logger = logger;
            _publisher = publisher;
            _messagingAppService = messagingAppService;
        }

        protected async Task<IActionResult> SendMessageAsync(string action, TDto payload, string logPrefix, string queueName)
        {
            var envelope = new GenericMessage<TDto>
            {
                Action = action,
                Payload = payload,
                MessagingId = 0
            };

            var messageRequest = JsonSerializer.Serialize(envelope);

            var messagingId = await _messagingAppService.AddAsync(new MessagingDto(
                action,
                queueName,
                messageRequest,
                false,
                string.Empty
            ));

            envelope.MessagingId = messagingId;

            _publisher.Publish(envelope, queueName);

            _logger.LogInformation("📤 '{Action}' message sent to queue, ID {Id}", action, messagingId);

            return Accepted(new { message = "Request queued successfully", id = messagingId });
        }

        protected async Task<IActionResult> GetResultAsyncInternal(long id, Func<string, string, object?> deserializeCallback)
        {
            try
            {
                var result = await _messagingAppService.GetByIdAsync(id);

                if (result is null)
                    return NotFound("Message not found.");

                if (!result.IsProcessed)
                    return Accepted(new { message = "Still in processing", id });

                var response = deserializeCallback(result.Action, result.MessageResponse);

                return Ok(new
                {
                    id = result.Id,
                    originalAction = result.Action,
                    processed = result.IsProcessed,
                    response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying message ID result {MessagingId}", id);
                return StatusCode(500, "Error getting message result.");
            }
        }
    }
}