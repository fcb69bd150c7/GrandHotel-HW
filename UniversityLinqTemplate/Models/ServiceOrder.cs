namespace UniversityLinqTemplate.Models;

public class ServiceOrder
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsPaid { get; set; } = true;
}

