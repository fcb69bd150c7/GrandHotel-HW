namespace UniversityLinqTemplate.Models;

public enum BookingStatus
{
    Confirmed,
    CheckedIn,
    Completed,
    Cancelled
}

public class Booking
{
    public int Id { get; set; }

    public int GuestId { get; set; }
    public Guest? Guest { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalCost { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public DateTime CreatedAt { get; set; }

    public List<ServiceOrder> ServiceOrders { get; set; } = new();
}

