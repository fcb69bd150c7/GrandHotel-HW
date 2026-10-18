namespace UniversityLinqTemplate.Models;

public class RoomCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePricePerNight { get; set; }
    public int Capacity { get; set; }

    public List<Room> Rooms { get; set; } = new();
}

