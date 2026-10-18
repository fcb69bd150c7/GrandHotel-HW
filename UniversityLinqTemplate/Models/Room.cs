namespace UniversityLinqTemplate.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool HasBalcony { get; set; }

    public int RoomCategoryId { get; set; }
    public RoomCategory? RoomCategory { get; set; }

    public List<Booking> Bookings { get; set; } = new();
}

