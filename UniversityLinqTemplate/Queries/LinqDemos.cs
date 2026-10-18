using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversityLinqTemplate.Data;
using UniversityLinqTemplate.Models;

namespace UniversityLinqTemplate.Queries;

public static class LinqDemos
{
    public static void DemoFilteringAndSorting(UniversityContext db)
    {
        Console.WriteLine("\n--- 1. Фильтрация и сортировка (Where, OrderBy, OrderByDescending) ---");

        Console.WriteLine("Свободные номера с балконом выше 1 этажа:");
        var availableRooms = db.Rooms
            .Include(r => r.RoomCategory)
            .Where(r => r.IsAvailable && r.HasBalcony && r.Floor > 1)
            .OrderBy(r => r.Floor)
            .ThenBy(r => r.RoomNumber)
            .ToList();

        foreach (var r in availableRooms)
        {
            Console.WriteLine($"  Номер {r.RoomNumber} (этаж {r.Floor}, {r.RoomCategory?.Name}) - {r.RoomCategory?.BasePricePerNight:C0}/ночь");
        }

        Console.WriteLine("\nБронирования по убыванию стоимости:");
        var sortedBookings = db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
            .Where(b => b.Status != BookingStatus.Cancelled)
            .OrderByDescending(b => b.TotalCost)
            .ToList();

        foreach (var b in sortedBookings)
        {
            Console.WriteLine($"  Бронь #{b.Id}: {b.Guest?.FullName} - номер {b.Room?.RoomNumber}, статус {b.Status}, сумма {b.TotalCost:N0} руб.");
        }
    }

    public static void DemoProjection(UniversityContext db)
    {
        Console.WriteLine("\n--- 2. Проекция (Select и анонимные типы) ---");

        var bookingSummaries = db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
                .ThenInclude(r => r!.RoomCategory)
            .Select(b => new
            {
                BookingId = b.Id,
                GuestName = b.Guest != null ? b.Guest.FullName : "Не указан",
                GuestPhone = b.Guest != null ? b.Guest.Phone : "-",
                RoomInfo = b.Room != null ? $"{b.Room.RoomNumber} ({b.Room.RoomCategory!.Name})" : "Без номера",
                Nights = (b.CheckOutDate - b.CheckInDate).Days,
                TotalCost = b.TotalCost,
                DailyRate = (b.CheckOutDate - b.CheckInDate).Days > 0
                    ? b.TotalCost / (b.CheckOutDate - b.CheckInDate).Days
                    : b.TotalCost
            })
            .OrderBy(b => b.BookingId)
            .ToList();

        foreach (var b in bookingSummaries)
        {
            Console.WriteLine($"  Бронь #{b.BookingId}: {b.GuestName} ({b.GuestPhone}) -> {b.RoomInfo}, {b.Nights} дн., {b.TotalCost:N0} руб. ({b.DailyRate:N0} руб./сутки)");
        }
    }

    public static void DemoAggregations(UniversityContext db)
    {
        Console.WriteLine("\n--- 3. Агрегатные вычисления (Count, Sum, Average, Min, Max) ---");

        int totalGuests = db.Guests.Count();
        int totalRooms = db.Rooms.Count();
        int activeRoomsCount = db.Rooms.Count(r => r.IsAvailable);

        int totalBookings = db.Bookings.Count();
        int completedBookings = db.Bookings.Count(b => b.Status == BookingStatus.Completed);

        decimal totalRevenue = db.Bookings
            .Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.CheckedIn)
            .Sum(b => b.TotalCost);

        decimal avgBookingCost = db.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Average(b => b.TotalCost);

        decimal maxBookingCost = db.Bookings.Max(b => b.TotalCost);
        decimal minBookingCost = db.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Min(b => b.TotalCost);

        int totalServicesCount = db.ServiceOrders.Count();
        decimal totalServicesRevenue = db.ServiceOrders.Where(s => s.IsPaid).Sum(s => s.Price);

        Console.WriteLine($"  Всего гостей: {totalGuests}");
        Console.WriteLine($"  Номерной фонд: {totalRooms} номеров (доступно: {activeRoomsCount})");
        Console.WriteLine($"  Всего броней: {totalBookings} (завершено: {completedBookings})");
        Console.WriteLine($"  Выручка от проживания: {totalRevenue:N0} руб.");
        Console.WriteLine($"  Средний чек бронирования: {avgBookingCost:N0} руб.");
        Console.WriteLine($"  Диапазон стоимости броней: от {minBookingCost:N0} до {maxBookingCost:N0} руб.");
        Console.WriteLine($"  Дополнительные услуги: {totalServicesCount} заказов на сумму {totalServicesRevenue:N0} руб.");
    }

    public static void DemoGrouping(UniversityContext db)
    {
        Console.WriteLine("\n--- 4. Группировка (GroupBy) ---");

        Console.WriteLine("Номера по категориям:");
        var roomsByCategory = db.Rooms
            .GroupBy(r => r.RoomCategory!.Name)
            .Select(g => new
            {
                CategoryName = g.Key,
                RoomsCount = g.Count(),
                AvailableCount = g.Count(r => r.IsAvailable),
                AvgFloor = g.Average(r => r.Floor),
                BalconyCount = g.Count(r => r.HasBalcony)
            })
            .OrderByDescending(g => g.RoomsCount)
            .ToList();

        foreach (var item in roomsByCategory)
        {
            Console.WriteLine($"  - {item.CategoryName}: {item.RoomsCount} комн. (свободно {item.AvailableCount}, с балконом {item.BalconyCount}, ср. этаж {item.AvgFloor:F1})");
        }

        Console.WriteLine("\nБронирования по статусам:");
        var bookingsByStatus = db.Bookings
            .GroupBy(b => b.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                TotalSum = g.Sum(b => b.TotalCost),
                AvgSum = g.Average(b => b.TotalCost)
            })
            .OrderByDescending(g => g.TotalSum)
            .ToList();

        foreach (var stat in bookingsByStatus)
        {
            Console.WriteLine($"  - {stat.Status}: {stat.Count} броней на сумму {stat.TotalSum:N0} руб. (средний чек {stat.AvgSum:N0} руб.)");
        }

        Console.WriteLine("\nПопулярность доп. услуг:");
        var servicesSummary = db.ServiceOrders
            .GroupBy(s => s.ServiceName)
            .Select(g => new
            {
                Service = g.Key,
                OrdersCount = g.Count(),
                TotalRevenue = g.Sum(s => s.Price)
            })
            .OrderByDescending(g => g.OrdersCount)
            .ThenByDescending(g => g.TotalRevenue)
            .ToList();

        foreach (var s in servicesSummary)
        {
            Console.WriteLine($"  - {s.Service}: {s.OrdersCount} заказов, выручка {s.TotalRevenue:N0} руб.");
        }
    }

    public static void DemoJoinsAndIncludes(UniversityContext db)
    {
        Console.WriteLine("\n--- 5. Жадная загрузка связей (Include, ThenInclude) ---");

        var guestsHistory = db.Guests
            .Include(g => g.Bookings)
                .ThenInclude(b => b.Room)
                    .ThenInclude(r => r!.RoomCategory)
            .Include(g => g.Bookings)
                .ThenInclude(b => b.ServiceOrders)
            .OrderBy(g => g.FullName)
            .ToList();

        foreach (var guest in guestsHistory)
        {
            Console.WriteLine($"\nГость: {guest.FullName} (скидка {guest.DiscountPercent}%, {guest.Phone})");

            if (!guest.Bookings.Any())
            {
                Console.WriteLine("  (нет активных бронирований)");
                continue;
            }

            foreach (var b in guest.Bookings)
            {
                Console.WriteLine($"  * Бронь #{b.Id}: комната {b.Room?.RoomNumber} ({b.Room?.RoomCategory?.Name}) c {b.CheckInDate:dd.MM} по {b.CheckOutDate:dd.MM}, статус {b.Status}, сумма {b.TotalCost:N0} руб.");

                foreach (var so in b.ServiceOrders)
                {
                    Console.WriteLine($"      - услуга: {so.ServiceName} ({so.Price:N0} руб.)");
                }
            }
        }
    }

    public static void DemoQuantifiersAndPaging(UniversityContext db)
    {
        Console.WriteLine("\n--- 6. Квантификаторы (Any, All) и пагинация (Take, Skip) ---");

        bool hasVipGuests = db.Guests.Any(g => g.DiscountPercent >= 15);
        Console.WriteLine($"  Есть ли VIP-гости со скидкой от 15%: {(hasVipGuests ? "Да" : "Нет")}");

        bool hasUnpaidServices = db.ServiceOrders.Any(s => !s.IsPaid);
        Console.WriteLine($"  Есть ли неоплаченные услуги: {(hasUnpaidServices ? "Да" : "Нет")}");

        bool allPresidentsHaveBalcony = db.Rooms
            .Where(r => r.RoomCategory!.Name == "Президентский люкс")
            .All(r => r.HasBalcony);
        Console.WriteLine($"  Все ли президентские номера с балконом: {(allPresidentsHaveBalcony ? "Да" : "Нет")}");

        Console.WriteLine("\nТоп-3 самых дорогих бронирований:");
        var top3Bookings = db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
                .ThenInclude(r => r!.RoomCategory)
            .OrderByDescending(b => b.TotalCost)
            .Take(3)
            .ToList();

        int rank = 1;
        foreach (var b in top3Bookings)
        {
            Console.WriteLine($"  {rank++}. {b.Guest?.FullName}: {b.Room?.RoomNumber} ({b.Room?.RoomCategory?.Name}), {b.TotalCost:N0} руб.");
        }

        int pageSize = 3;
        int pageIndex = 1;
        Console.WriteLine($"\nГости на странице 2 (по {pageSize} на страницу):");

        var pagedGuests = db.Guests
            .OrderBy(g => g.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList();

        foreach (var g in pagedGuests)
        {
            Console.WriteLine($"  - [id {g.Id}] {g.FullName}, скидка {g.DiscountPercent}%, {g.Email}");
        }
    }
}
