using System.ComponentModel.DataAnnotations;

namespace Order.API.Models.Entities
{
    public class OrderOutbox //Aslında burası OutboxTable
    {
        public int  Id { get; set; }
        public DateTime OccuredOn { get; set; } // Eventin oluşturulacağı zaman
        public DateTime? ProcessedDate { get; set; } //Event'in işlenme zamanı. Order.Outbox.Table.Publisher.Service i bu kolonu boş olan dataları alıcak işleyecek.Çünkü ProcessedDate'i dolu olanlar alınıp işlenmiştir zaten adı üstünde işlenme zamanı.
        public string Type { get; set; } // Outbox Table'a kaydedilecek eventlerin tipi(mesela OrderCreatedEvent)
        public string Payload { get; set; } // Göndereceğimiz eventin bilgileri

    }
}
