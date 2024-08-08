using EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Consumers;
using EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Db;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Configuration.ConsumerId = args.Count() > 0 ? args[0] : string.Empty;
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    _ = services.AddDbContext<ConsumerAppDbContext>(c =>
                    {
                        var connectionString = hostContext.Configuration.GetConnectionString("SqlServer");
                        c.UseSqlServer(connectionString);

                    });

                    _ = services.AddHostedService<RecreateDatabaseHostedService<ConsumerAppDbContext>>();

                    _ = services.AddMassTransit(x =>
                    {
                        x.AddEntityFrameworkOutbox<ConsumerAppDbContext>(o =>
                        {
                            o.UseSqlServer();

                            // Inbox detects duplicated messages after message received for this long. Default is 30 MINUTES.
                            o.DuplicateDetectionWindow = TimeSpan.FromHours(10);

                            // Messages will stay in inbox (duplicated messages detection will work for them forever).
                            //o.DisableInboxCleanupService();
                        });

                        // Add a consumer for a message type.
                        x.AddConsumer<MessageAConsumer>();

                        x.UsingActiveMq((ctx, cfg) =>
                        {
                            cfg.Host("localhost", h =>
                            {
                                h.Username("artemis");
                                h.Password("artemis");
                                h.UseSsl(false);
                            });

                            // Add a receive endpoint (queue).
                            cfg.ReceiveEndpoint("a-queue", e =>
                            {
                                // Add a consumer to the endpoint.
                                e.ConfigureConsumer<MessageAConsumer>(ctx);

                                // This setting can limit how many consumer thread can run simultaneously. When using MySql, there are concurrency
                                // issues (deadlocks) due to a InnoDB specific behavior: https://dev.mysql.com/doc/refman/8.4/en/innodb-locks-set.html
                                // A discussion about this issues: https://github.com/MassTransit/MassTransit/discussions/4796
                                // MySQL/MariaDB should not be used, or isolation level should be lowered.
                                //e.ConcurrentMessageLimit = 1;

                                // Add this config if Inbox/Outbox functionality (called Counsumer Outbox in MT documentation) is needed for this endpoint.
                                // Inbox part of the Counsumer Outbox is used for detecting duplicated received messages. Inbox cannot be added separately
                                // for the consumer, only along with Outbox. Inbox settings can be configured in x.AddEntityFrameworkOutbox<ConsumerAppDbContext>().
                                // Outbox part of the Counsumer Outbox provides a similar behavior like the Bus Outbox, but it works only for a consumer,
                                // who wants to send/publish message. The most important difference to Bus Outbox is that while in Bus Outbox the transaction is
                                // committed when dbCtx.SaveChanges() is called explicitly, but when using Consumer Outbox's outbox, the transaction is handled
                                // by MT, which means, no ctx.SaveChanges() is needed, it will be called implicitly by the MT after Consume() method is returned.
                                e.UseEntityFrameworkOutbox<ConsumerAppDbContext>(ctx);
                            });

                            // This setting seems unneccessary.
                            //cfg.EnableArtemisCompatibility();
                        });
                    });
                });
        }
    }
}
