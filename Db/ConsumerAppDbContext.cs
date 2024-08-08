using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EIH.Common.POC.Common.MassTransit.TransactionOutbox.Consumer.Db
{
    public class ConsumerAppDbContext :
        DbContext
    {
        public ConsumerAppDbContext(DbContextOptions<ConsumerAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //MapMessageA(modelBuilder);

            // Consumer Outbox configuration for Transaction Outbox
            modelBuilder.AddTransactionalOutboxEntities();
        }

        //static void MapMessageA(ModelBuilder modelBuilder)
        //{
        //    EntityTypeBuilder<MessageA> registration = modelBuilder.Entity<MessageA>();

        //    registration.Property(x => x.Id);
        //    registration.HasKey(x => x.Id);

        //    registration.Property(x => x.Text);

        //    registration.HasIndex(x => new { x.Id }).IsUnique();
        //}
    }
}
