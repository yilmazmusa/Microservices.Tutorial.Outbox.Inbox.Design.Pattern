using Quartz;

namespace Order.Outbox.Table.Publisher.Service.Jobs
{
    public class OrderOutboxPublishJob : IJob // Outbox table daki publish edilecek/işlenecek bütün işlemleri/mesajalrı bu Job üzerinden yürütücez.
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("tetikledi..." + DateTime.UtcNow.Second);
        }
    }
}
