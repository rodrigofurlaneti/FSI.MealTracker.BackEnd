using FSI.MealTracker.Application.Dtos;
using FSI.MealTracker.Application.Interfaces;
using FSI.MealTracker.Application.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FSI.MealTracker.Worker
{
    public class ConsumptionConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public ConsumptionConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _config = config;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = "consumption-queue";

            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMq:Host"] ?? "localhost",
                UserName = _config["RabbitMq:User"] ?? "guest",
                Password = _config["RabbitMq:Password"] ?? "guest"
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine("📥 Message received from RabbitMQ:");
                    Console.WriteLine(message);

                    var envelope = JsonSerializer.Deserialize<ConsumptionMessage>(
                        message,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (envelope == null)
                    {
                        Console.WriteLine("❌ Envelope is null. Check message format.");
                        return;
                    }

                    Console.WriteLine($"✔ Action received: {envelope.Action}");

                    Console.WriteLine($"✔ Payload: {JsonSerializer.Serialize(envelope.Payload)}");

                    using var scope = _scopeFactory.CreateScope();

                    var service = scope.ServiceProvider.GetRequiredService<IConsumptionAppService>();

                    var messagingService = scope.ServiceProvider.GetRequiredService<IMessagingAppService>();

                    long? createdId = null;

                    bool isDone = false;

                    IEnumerable<ConsumptionDto> listConsumption = null;

                    ConsumptionDto expenseCategory = null;

                    switch (envelope.Action.ToLowerInvariant())
                    {
                        case "create":
                            createdId = await service.AddAsync(envelope.Payload);
                            break;
                        case "getall":
                            listConsumption = await service.GetAllAsync();
                            break;
                        case "getbyid":
                            expenseCategory = await service.GetByIdAsync(envelope.Payload.Id);
                            break;
                        case "update":
                            isDone = await service.UpdateAsync(envelope.Payload);
                            break;
                        case "delete":
                            isDone = await service.DeleteAsync(envelope.Payload.Id);
                            break;
                        default:
                            Console.WriteLine($"⚠ Action not recognized: {envelope.Action}");
                            break;
                    }

                    await ProcessedMessageAsync(messagingService, envelope, queueName, createdId, isDone, listConsumption, expenseCategory);

                    // ✅ Manual confirmation that the message was processed successfully
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    // ❌ Não dar o Ack -> mensagem permanece na fila
                    // 🔴 Logar erro de parsing ou de serviço
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // 🔄 Loop para manter o serviço ativo enquanto não for cancelado
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

                // Cleanup opcional:
                channel.Close();
                connection.Close();
            }, stoppingToken);
        }

        private async Task ProcessedMessageAsync(IMessagingAppService messagingService, ConsumptionMessage envelope, string queueName, long? createdId, bool isDone,
            IEnumerable<ConsumptionDto> listConsumption, ConsumptionDto expenseCategory)
        {
            // ✅ The processing status of the record in the database to processed type create
            if (envelope.MessagingId > 0 && envelope.Action.Equals("create", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessedMessageCreateAsync(messagingService, envelope, queueName, createdId);
            }

            // ✅ The processing status of the record in the database to processed type get all
            if (envelope.MessagingId > 0 && envelope.Action.Equals("getall", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessedMessageGetAllAsync(messagingService, envelope, queueName, listConsumption);
            }

            // ✅ The processing status of the record in the database to processed type get by id
            if (envelope.MessagingId > 0 && envelope.Action.Equals("getbyid", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessedMessageGetByIdAsync(messagingService, envelope, queueName, expenseCategory);
            }

            // ✅ The processing status of the record in the database to processed type update
            if (envelope.MessagingId > 0 && envelope.Action.Equals("update", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessedMessageUpdateAsync(messagingService, envelope, queueName, isDone);
            }

            // ✅ The processing status of the record in the database to processed type delete
            if (envelope.MessagingId > 0 && envelope.Action.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessedMessageDeleteAsync(messagingService, envelope, queueName, isDone);
            }
        }

        private async Task ProcessedMessageCreateAsync(IMessagingAppService messagingService, ConsumptionMessage envelope,
            string queueName, long? createdId)
        {
            if (createdId != null)
            {
                envelope.Payload.Id = createdId.Value;
                envelope.Payload.UpdatedAt = DateTime.Now;

                var updatedContentRequest = JsonSerializer.Serialize(envelope);

                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "Create",
                    QueueName = queueName,
                    MessageRequest = updatedContentRequest,
                    MessageResponse = string.Empty,
                    IsProcessed = true,
                    ErrorMessage = string.Empty,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                });

                Console.WriteLine($"✔ Message ID {envelope.MessagingId} marked as processed.");
            }
            else
            {
                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "Create",
                    QueueName = queueName,
                    MessageRequest = JsonSerializer.Serialize(envelope),
                    MessageResponse = string.Empty,
                    IsProcessed = false,
                    ErrorMessage = "Failed to insert expense category into database.",
                    UpdatedAt = DateTime.Now,
                    IsActive = false
                });

                Console.WriteLine($"❌ Failed to process message ID {envelope.MessagingId}: creation returned null.");
            }
        }

        private async Task ProcessedMessageGetAllAsync(IMessagingAppService messagingService, ConsumptionMessage envelope,
            string queueName, IEnumerable<ConsumptionDto> listConsumption)
        {
            if (listConsumption != null)
            {
                var updatedResponse = JsonSerializer.Serialize(listConsumption);

                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "GetAll",
                    QueueName = queueName,
                    MessageRequest = string.Empty,
                    MessageResponse = updatedResponse,
                    IsProcessed = true,
                    ErrorMessage = string.Empty,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                });

                Console.WriteLine($"✔ Message ID {envelope.MessagingId} marked as processed.");
            }
            else
            {
                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "GetAll",
                    QueueName = queueName,
                    MessageRequest = string.Empty,
                    MessageResponse = JsonSerializer.Serialize(listConsumption),
                    IsProcessed = false,
                    ErrorMessage = "Failed to insert expense category into database.",
                    UpdatedAt = DateTime.Now,
                    IsActive = false
                });

                Console.WriteLine($"❌ Failed to process message ID {envelope.MessagingId}: creation returned null.");
            }
        }

        private async Task ProcessedMessageGetByIdAsync(IMessagingAppService messagingService, ConsumptionMessage envelope,
            string queueName, ConsumptionDto expenseCategory)
        {
            if (expenseCategory != null)
            {
                var updatedRequest = JsonSerializer.Serialize(envelope);

                var updatedResponse = JsonSerializer.Serialize(expenseCategory);

                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "GetById",
                    QueueName = queueName,
                    MessageRequest = updatedRequest,
                    MessageResponse = updatedResponse,
                    IsProcessed = true,
                    ErrorMessage = string.Empty,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                });

                Console.WriteLine($"✔ Message ID {envelope.MessagingId} marked as processed.");
            }
            else
            {
                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "GetById",
                    QueueName = queueName,
                    MessageRequest = JsonSerializer.Serialize(envelope),
                    MessageResponse = JsonSerializer.Serialize(expenseCategory),
                    IsProcessed = false,
                    ErrorMessage = "Failed to insert expense category into database.",
                    UpdatedAt = DateTime.Now,
                    IsActive = false
                });

                Console.WriteLine($"❌ Failed to process message ID {envelope.MessagingId}: get by id returned null.");
            }
        }

        private async Task ProcessedMessageUpdateAsync(IMessagingAppService messagingService, ConsumptionMessage envelope,
            string queueName, bool isDone)
        {
            if (isDone)
            {
                envelope.Payload.UpdatedAt = DateTime.Now;

                var updatedContent = JsonSerializer.Serialize(envelope);

                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "Update",
                    QueueName = queueName,
                    MessageRequest = updatedContent,
                    MessageResponse = string.Empty,
                    IsProcessed = true,
                    ErrorMessage = string.Empty,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                });

                Console.WriteLine($"✔ Message ID {envelope.MessagingId} marked as processed.");
            }
            else
            {
                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "Update",
                    QueueName = queueName,
                    MessageRequest = JsonSerializer.Serialize(envelope),
                    MessageResponse = string.Empty,
                    IsProcessed = false,
                    ErrorMessage = "Failed to insert expense category into database.",
                    UpdatedAt = DateTime.Now,
                    IsActive = false
                });

                Console.WriteLine($"❌ Failed to process message ID {envelope.MessagingId}: creation returned null.");
            }
        }

        private async Task ProcessedMessageDeleteAsync(IMessagingAppService messagingService, ConsumptionMessage envelope,
            string queueName, bool isDone)
        {
            if (isDone)
            {
                envelope.Payload.UpdatedAt = DateTime.Now;

                var updatedContent = JsonSerializer.Serialize(envelope);

                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "Delete",
                    QueueName = queueName,
                    MessageRequest = updatedContent,
                    MessageResponse = string.Empty,
                    IsProcessed = true,
                    ErrorMessage = string.Empty,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                });

                Console.WriteLine($"✔ Message ID {envelope.MessagingId} marked as processed.");
            }
            else
            {
                await messagingService.UpdateAsync(new MessagingDto
                {
                    Id = envelope.MessagingId,
                    Action = "Delete",
                    QueueName = queueName,
                    MessageRequest = JsonSerializer.Serialize(envelope),
                    MessageResponse = string.Empty,
                    IsProcessed = false,
                    ErrorMessage = "Failed to insert expense category into database.",
                    UpdatedAt = DateTime.Now,
                    IsActive = false
                });

                Console.WriteLine($"❌ Failed to process message ID {envelope.MessagingId}: creation returned null.");
            }
        }
    }
}