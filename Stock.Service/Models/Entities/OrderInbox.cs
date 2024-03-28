using System.ComponentModel.DataAnnotations;

namespace Stock.Service.Models.Entities
{
    public class OrderInbox //Inbox table aslında burası
    {
        [Key]
        public int Id { get; set; }
        public bool Processed { get; set; }
        public string Payload { get; set; }
    }
}
