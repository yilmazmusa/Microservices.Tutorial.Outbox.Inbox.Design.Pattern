using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.Service.Models.Contexts;
using Stock.Service.Models.Entities;
using System.Text.Json;

namespace Stock.Service.Consumers
{
    public class OrderCreatedEventConsumer(StockDbContext stockDbContext) : IConsumer<OrderCreatedEvent>
    {


        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {

            await stockDbContext.OrderInboxes.AddAsync(new()
            {
                Processed = false,
                Payload = JsonSerializer.Serialize(context.Message) //Serilize ederken tip önemli değildir.
            });

            await stockDbContext.SaveChangesAsync();


            List<OrderInbox> orderInboxes = await stockDbContext.OrderInboxes
                .Where(x => x.Processed == false)
                .ToListAsync();

            foreach (var orderInbox in orderInboxes)
            {
                if (orderInbox != null)
                {
                    OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload); //Derilize ederken tip önemli.
                    Console.WriteLine($"{orderCreatedEvent.OrderId} order id değerine karşılık olan siparişin stok işlemleri başarıyla tamamlanmıştır.");

                    orderInbox.Processed = true; // işlenen eventin/mesajın değerini false tan true ya çektik.

                    await stockDbContext.SaveChangesAsync(); // her satır için yapılan Processed = true işlemi için savechanges yapıyorum
                }
            }

        }













































        //public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        //{
        //    var result = await stockDbContext.OrderInboxes.AnyAsync(i => i.IdempotentToken == context.Message.IdempotentToken);
        //    if (!result)
        //    {

        //        await stockDbContext.OrderInboxes.AddAsync(new()
        //        {
        //            Processed = false,
        //            Payload = JsonSerializer.Serialize(context.Message),
        //            IdempotentToken = context.Message.IdempotentToken
        //        });

        //        await stockDbContext.SaveChangesAsync();
        //    }


        //    List<OrderInbox> orderInboxes = await stockDbContext.OrderInboxes
        //        .Where(i => i.Processed == false)
        //        .ToListAsync();
        //    foreach (var orderInbox in orderInboxes)
        //    {
        //        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload);
        //        Console.WriteLine($"{orderCreatedEvent.OrderId} order id değerine karşılık olan siparişin stok işlemleri başarıyla tamamlanmıştır.");
        //        orderInbox.Processed = true;
        //        await stockDbContext.SaveChangesAsync();
        //    }
        //}
    }
}
