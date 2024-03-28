using MassTransit;
using Order.Outbox.Table.Publisher.Service.Entities;
using Quartz;
using Shared.Events;
using System.Text.Json;


namespace Order.Outbox.Table.Publisher.Service.Jobs
{
    public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint) : IJob // Outbox table(OrderOutbox) daki publish edilecek/işlenecek bütün işlemleri/mesajalrı bu Job üzerinden yürütücez.
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderOutboxSingletonDatabase.DataReaderState) // db nin durumu true ise yani hazır ise
            {
                OrderOutboxSingletonDatabase.DataReaderBusy(); // işlem yapacağımız için db nin durumunu yoğuna çektik
                List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM ORDEROUTBOXES WHERE PROCESSEDDATE IS NULL ORDER BY OCCUREDON ASC")).ToList();// db de ProcessedDate kolonu null olan dataları getiridk

                foreach (var orderOutbox in orderOutboxes)
                {
                    if (orderOutbox.Type == nameof(OrderCreatedEvent))
                    {
                        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload); //Json olarak payloada bastığımız veriyi bu sefer deserilize edip orderCreatedEvent'e bastık

                        if (orderCreatedEvent != null)
                        {
                            await publishEndpoint.Publish(orderCreatedEvent); // orderCreatedEvent'i MessageBroker ile publish ettik.

                            //ProcessedDate'i boş olan datayı işledik yukarda ve şimdide ProcessedDate kolonunu işlendiği tarihle doldurduk.
                            OrderOutboxSingletonDatabase.ExecuteAsync($@"UPDATE  OrderOutboxes SET ProcessedDate = GETDATE() WHERE Id = '{orderOutbox.Id}'");
                        }
                    }
                }
                OrderOutboxSingletonDatabase.DataReaderReady(); // işlemlerimizi yaptık çıkarken db nin durumunu tekrar hazıra getiriyoruz.
                await Console.Out.WriteLineAsync("Order Outbox table checked !");
            }
        }
    }
}
