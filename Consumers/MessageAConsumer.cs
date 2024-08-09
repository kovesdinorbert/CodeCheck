using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Consumers
{
    public class MessageAConsumer : IConsumer<MessageA>
    {
        readonly ILogger<MessageAConsumer> _logger;
        private List<int> _list;
        public MessageAConsumer(ILogger<MessageAConsumer> logger)
        {
            _logger = logger;
        }

        private void I()
        {
            _list = new List<int>();
        }

        public Task Consume(ConsumeContext<MessageA> context)
        {
            //TODO
            //Task.Delay(1000).GetAwaiter().GetResult();
            int needRemove = 0;
            _list.Add(3);
            _logger.LogInformation("Consumer {ConsumerId} Received Text: {Text}", Configuration.ConsumerId != "" ? Configuration.ConsumerId : "0", context.Message.Text);

            return Task.CompletedTask;
        }
    }
}
