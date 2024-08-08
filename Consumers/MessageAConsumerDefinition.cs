using EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Db;
using MassTransit;

namespace EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Consumers
{
    public class MessageAConsumerDefinition : ConsumerDefinition<MessageAConsumer>
    {
        protected override void ConfigureConsumer
            (IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<MessageAConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            //endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000));

            endpointConfigurator.UseEntityFrameworkOutbox<ConsumerAppDbContext>(context);

        }
    }
}