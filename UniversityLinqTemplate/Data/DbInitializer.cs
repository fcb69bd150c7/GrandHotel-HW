using System;
using System.Collections.Generic;
using UniversityLinqTemplate.Models;

namespace UniversityLinqTemplate.Data;

public static class DbInitializer
{
    public static void Initialize(UniversityContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (context.RoomCategories.Any())
            return;

        // Категории номеров
        var catStandard = new RoomCategory
        {
            Name = "Стандарт",
            Description = "Однокомнатный номер со всеми удобствами",
            BasePricePerNight = 2800m,
            Capacity = 2
        };

        var catComfort = new RoomCategory
        {
            Name = "Комфорт",
            Description = "Улучшенный номер с рабочей зоной и чайной станцией",
            BasePricePerNight = 4500m,
            Capacity = 2
        };

        var catFamily = new RoomCategory
        {
            Name = "Семейный",
            Description = "Двухкомнатный номер для семей с детьми",
            BasePricePerNight = 6800m,
            Capacity = 4
        };

        var catJuniorSuite = new RoomCategory
        {
            Name = "Полулюкс",
            Description = "Номер с панорамными окнами и зоной отдыха",
            BasePricePerNight = 9200m,
            Capacity = 2
        };

        var catPresident = new RoomCategory
        {
            Name = "Президентский люкс",
            Description = "Апартаменты с джакузи и персональным сервисом",
            BasePricePerNight = 25000m,
            Capacity = 4
        };

        context.RoomCategories.AddRange(catStandard, catComfort, catFamily, catJuniorSuite, catPresident);

        // Номера
        var r101 = new Room { RoomNumber = "101", Floor = 1, IsAvailable = true, HasBalcony = false, RoomCategory = catStandard };
        var r102 = new Room { RoomNumber = "102", Floor = 1, IsAvailable = true, HasBalcony = false, RoomCategory = catStandard };
        var r103 = new Room { RoomNumber = "103", Floor = 1, IsAvailable = true, HasBalcony = true, RoomCategory = catComfort };

        var r201 = new Room { RoomNumber = "201", Floor = 2, IsAvailable = true, HasBalcony = true, RoomCategory = catComfort };
        var r202 = new Room { RoomNumber = "202", Floor = 2, IsAvailable = true, HasBalcony = true, RoomCategory = catFamily };
        var r203 = new Room { RoomNumber = "203", Floor = 2, IsAvailable = true, HasBalcony = false, RoomCategory = catFamily };

        var r301 = new Room { RoomNumber = "301", Floor = 3, IsAvailable = true, HasBalcony = true, RoomCategory = catJuniorSuite };
        var r302 = new Room { RoomNumber = "302", Floor = 3, IsAvailable = true, HasBalcony = true, RoomCategory = catJuniorSuite };

        var r401 = new Room { RoomNumber = "401", Floor = 4, IsAvailable = true, HasBalcony = true, RoomCategory = catPresident };
        var r402 = new Room { RoomNumber = "402", Floor = 4, IsAvailable = false, HasBalcony = true, RoomCategory = catPresident };

        context.Rooms.AddRange(r101, r102, r103, r201, r202, r203, r301, r302, r401, r402);

        // Гости
        var g1 = new Guest
        {
            FullName = "Алексей Смирнов",
            PassportNumber = "4512 889901",
            Phone = "+7-916-111-22-33",
            Email = "a.smirnov@mail.ru",
            DiscountPercent = 10,
            RegisteredAt = new DateTime(2024, 5, 12)
        };

        var g2 = new Guest
        {
            FullName = "Екатерина Соколова",
            PassportNumber = "4515 334455",
            Phone = "+7-926-222-33-44",
            Email = "e.sokolova@gmail.com",
            DiscountPercent = 15,
            RegisteredAt = new DateTime(2023, 11, 20)
        };

        var g3 = new Guest
        {
            FullName = "Дмитрий Морозов",
            PassportNumber = "4509 778899",
            Phone = "+7-903-333-44-55",
            Email = "d.morozov@yandex.ru",
            DiscountPercent = 5,
            RegisteredAt = new DateTime(2025, 2, 14)
        };

        var g4 = new Guest
        {
            FullName = "Анна Васильева",
            PassportNumber = "4518 112233",
            Phone = "+7-915-444-55-66",
            Email = "a.vasilieva@rambler.ru",
            DiscountPercent = 0,
            RegisteredAt = new DateTime(2025, 8, 1)
        };

        var g5 = new Guest
        {
            FullName = "Михаил Кузнецов",
            PassportNumber = "4520 667788",
            Phone = "+7-977-555-66-77",
            Email = "m.kuznetsov@corp.ru",
            DiscountPercent = 10,
            RegisteredAt = new DateTime(2024, 9, 10)
        };

        var g6 = new Guest
        {
            FullName = "Ольга Новикова",
            PassportNumber = "4511 990011",
            Phone = "+7-999-666-77-88",
            Email = "o.novikova@inbox.ru",
            DiscountPercent = 0,
            RegisteredAt = new DateTime(2026, 1, 15)
        };

        var g7 = new Guest
        {
            FullName = "Сергей Волков",
            PassportNumber = "4507 445566",
            Phone = "+7-905-777-88-99",
            Email = "s.volkov@outlook.com",
            DiscountPercent = 5,
            RegisteredAt = new DateTime(2025, 10, 5)
        };

        context.Guests.AddRange(g1, g2, g3, g4, g5, g6, g7);

        // Бронирования
        var now = DateTime.Now;

        var b1 = new Booking
        {
            Guest = g1,
            Room = r103,
            CheckInDate = now.AddDays(-15),
            CheckOutDate = now.AddDays(-12),
            NumberOfGuests = 2,
            TotalCost = 12150m,
            Status = BookingStatus.Completed,
            CreatedAt = now.AddDays(-20)
        };

        var b2 = new Booking
        {
            Guest = g2,
            Room = r401,
            CheckInDate = now.AddDays(-8),
            CheckOutDate = now.AddDays(-6),
            NumberOfGuests = 2,
            TotalCost = 42500m,
            Status = BookingStatus.Completed,
            CreatedAt = now.AddDays(-18)
        };

        var b3 = new Booking
        {
            Guest = g3,
            Room = r202,
            CheckInDate = now.AddDays(-2),
            CheckOutDate = now.AddDays(2),
            NumberOfGuests = 3,
            TotalCost = 25840m,
            Status = BookingStatus.CheckedIn,
            CreatedAt = now.AddDays(-10)
        };

        var b4 = new Booking
        {
            Guest = g4,
            Room = r101,
            CheckInDate = now.AddDays(-5),
            CheckOutDate = now.AddDays(-3),
            NumberOfGuests = 1,
            TotalCost = 5600m,
            Status = BookingStatus.Completed,
            CreatedAt = now.AddDays(-7)
        };

        var b5 = new Booking
        {
            Guest = g5,
            Room = r301,
            CheckInDate = now.AddDays(3),
            CheckOutDate = now.AddDays(6),
            NumberOfGuests = 2,
            TotalCost = 24840m,
            Status = BookingStatus.Confirmed,
            CreatedAt = now.AddDays(-1)
        };

        var b6 = new Booking
        {
            Guest = g1,
            Room = r302,
            CheckInDate = now.AddDays(10),
            CheckOutDate = now.AddDays(12),
            NumberOfGuests = 2,
            TotalCost = 16560m,
            Status = BookingStatus.Confirmed,
            CreatedAt = now.AddDays(-2)
        };

        var b7 = new Booking
        {
            Guest = g6,
            Room = r102,
            CheckInDate = now.AddDays(-4),
            CheckOutDate = now.AddDays(-3),
            NumberOfGuests = 1,
            TotalCost = 2800m,
            Status = BookingStatus.Cancelled,
            CreatedAt = now.AddDays(-6)
        };

        var b8 = new Booking
        {
            Guest = g2,
            Room = r201,
            CheckInDate = now.AddDays(-1),
            CheckOutDate = now.AddDays(4),
            NumberOfGuests = 2,
            TotalCost = 19125m,
            Status = BookingStatus.CheckedIn,
            CreatedAt = now.AddDays(-5)
        };

        context.Bookings.AddRange(b1, b2, b3, b4, b5, b6, b7, b8);

        // Дополнительные услуги
        context.ServiceOrders.AddRange(
            new ServiceOrder { Booking = b1, ServiceName = "Завтрак 'Шведский стол'", Price = 1200m, OrderDate = now.AddDays(-14), IsPaid = true },
            new ServiceOrder { Booking = b1, ServiceName = "Трансфер из аэропорта", Price = 2500m, OrderDate = now.AddDays(-15), IsPaid = true },
            new ServiceOrder { Booking = b2, ServiceName = "Спа-комплекс Премиум", Price = 9500m, OrderDate = now.AddDays(-7), IsPaid = true },
            new ServiceOrder { Booking = b2, ServiceName = "Рум-сервис: Ужин от шефа", Price = 6400m, OrderDate = now.AddDays(-7), IsPaid = true },
            new ServiceOrder { Booking = b2, ServiceName = "Индивидуальная экскурсия", Price = 12000m, OrderDate = now.AddDays(-6), IsPaid = true },
            new ServiceOrder { Booking = b3, ServiceName = "Детский клуб и анимация", Price = 3000m, OrderDate = now.AddDays(-1), IsPaid = true },
            new ServiceOrder { Booking = b3, ServiceName = "Завтрак 'Шведский стол' (семейный)", Price = 2800m, OrderDate = now.AddDays(-1), IsPaid = true },
            new ServiceOrder { Booking = b4, ServiceName = "Аренда халата и тапочек", Price = 500m, OrderDate = now.AddDays(-5), IsPaid = true },
            new ServiceOrder { Booking = b5, ServiceName = "Подземный паркинг", Price = 1500m, OrderDate = now.AddDays(-1), IsPaid = false },
            new ServiceOrder { Booking = b8, ServiceName = "Спа-процедура 'Релакс'", Price = 4500m, OrderDate = now, IsPaid = true },
            new ServiceOrder { Booking = b8, ServiceName = "Химчистка одежды", Price = 1800m, OrderDate = now, IsPaid = true }
        );

        context.SaveChanges();
    }
}
