using System.Text.Json.Serialization;

namespace BookStore.Entities;

public enum OrderStatus { PENDING, COMPLETED, CANCELLED }

public class Order
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    // [JsonConverter(typeof(JsonStringEnumConverter))] //enum'ı string olarak kabul etmesini sağlıyormuş 
    // Postamnde test ettim. string olarak kabul etmiyor.rakam olarak vermek gerekiyor..
    public OrderStatus Status { get; set; } = OrderStatus.PENDING;
}

