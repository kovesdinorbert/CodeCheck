using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Consumers
{
    public class MessageAConsumer : IConsumer<MessageA>
    {
        readonly ILogger<MessageAConsumer> _logger;
        public MessageAConsumer(ILogger<MessageAConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<MessageA> context)
        {
            //Task.Delay(1000).GetAwaiter().GetResult();
            _logger.LogInformation("Consumer {ConsumerId} Received Text: {Text}", Configuration.ConsumerId != "" ? Configuration.ConsumerId : "0", context.Message.Text);

            return Task.CompletedTask;
        }
    }
}
