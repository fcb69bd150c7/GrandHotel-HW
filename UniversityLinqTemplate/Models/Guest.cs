namespace UniversityLinqTemplate.Models;

public class Guest
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int DiscountPercent { get; set; }
    public DateTime RegisteredAt { get; set; }

    public List<Booking> Bookings { get; set; } = new();
}

