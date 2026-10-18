# Лабораторная работа: LINQ и Entity Framework Core

* **Тема:** Гостиничный комплекс GrandHotel
* **Стек:** C#, .NET 10, Entity Framework Core (Code-First), SQLite

Проект реализует базу данных отеля для учёта номеров, постояльцев, бронирований и дополнительных услуг, а также набор практических запросов на LINQ (Method Syntax и Query Syntax).

---

## Структура базы данных

База данных отеля состоит из 5 связанных таблиц:
* **RoomCategory** - категории номеров (название, описание, базовая цена за ночь, вместимость);
* **Room** - сами номера (номер комнаты, этаж, статус доступности, наличие балкона, ссылка на категорию);
* **Guest** - постояльцы (ФИО, паспорт, телефон, почта, скидка 0–15%);
* **Booking** - бронирования (даты заезда и выезда, количество гостей, итоговая стоимость, статус);
* **ServiceOrder** - доп. услуги к бронированию (завтрак, спа, трансфер и т.д.).

### Связи:
* У одной категории может быть несколько номеров (1 ко многим);
* Один гость может оформить несколько бронирований (1 ко многим);
* К одному номеру привязано множество броней по разным датам (1 ко многим);
* К каждому бронированию можно добавить несколько услуг (1 ко многим).

---

## Описание LINQ-запросов

Все примеры запросов собраны в файле `Queries/LinqDemos.cs`:

### 1. Фильтрация и сортировка (`Where`, `OrderBy`, `OrderByDescending`, `ThenBy`)
* Выборка свободных номеров с балконом выше первого этажа:
```csharp
var availableRooms = db.Rooms
    .Include(r => r.RoomCategory)
    .Where(r => r.IsAvailable && r.HasBalcony && r.Floor > 1)
    .OrderBy(r => r.Floor)
    .ThenBy(r => r.RoomNumber)
    .ToList();
```
* Сортировка всех неотменённых бронирований по убыванию чека:
```csharp
var sortedBookings = db.Bookings
    .Include(b => b.Guest)
    .Include(b => b.Room)
    .Where(b => b.Status != BookingStatus.Cancelled)
    .OrderByDescending(b => b.TotalCost)
    .ToList();
```

### 2. Проекция (`Select` и анонимные типы)
* Формирование сводки по бронированию с вычислением суток и стоимости за ночь:
```csharp
var bookingSummaries = db.Bookings
    .Include(b => b.Guest)
    .Include(b => b.Room)
        .ThenInclude(r => r!.RoomCategory)
    .Select(b => new
    {
        BookingId = b.Id,
        GuestName = b.Guest != null ? b.Guest.FullName : "Не указан",
        RoomInfo = b.Room != null ? $"{b.Room.RoomNumber} ({b.Room.RoomCategory!.Name})" : "Без номера",
        Nights = (b.CheckOutDate - b.CheckInDate).Days,
        TotalCost = b.TotalCost,
        DailyRate = (b.CheckOutDate - b.CheckInDate).Days > 0
            ? b.TotalCost / (b.CheckOutDate - b.CheckInDate).Days
            : b.TotalCost
    })
    .OrderBy(b => b.BookingId)
    .ToList();
```

### 3. Агрегатные операции (`Count`, `Sum`, `Average`, `Max`, `Min`)
* Подсчет выручки, среднего чека, минимальной и максимальной цены бронирования:
```csharp
int totalGuests = db.Guests.Count();
decimal totalRevenue = db.Bookings
    .Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.CheckedIn)
    .Sum(b => b.TotalCost);
decimal avgBookingCost = db.Bookings
    .Where(b => b.Status != BookingStatus.Cancelled)
    .Average(b => b.TotalCost);
decimal maxBookingCost = db.Bookings.Max(b => b.TotalCost);
```

### 4. Группировка (`GroupBy`)
* Группировка комнат по категориям со статистикой (сколько комнат, сколько свободно, средний этаж):
```csharp
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
```
* Группировка бронирований по статусам (Completed, CheckedIn, Confirmed, Cancelled).
* Группировка доп. услуг по названию с подсчётом выручки и количества заказов.

### 5. Жадная загрузка связей (`Include`, `ThenInclude`)
* Загрузка всей цепочки `Гость -> Бронирования -> Номер -> Категория номера` и `Бронирования -> Доп. услуги` одним запросом:
```csharp
var guestsHistory = db.Guests
    .Include(g => g.Bookings)
        .ThenInclude(b => b.Room)
            .ThenInclude(r => r!.RoomCategory)
    .Include(g => g.Bookings)
        .ThenInclude(b => b.ServiceOrders)
    .OrderBy(g => g.FullName)
    .ToList();
```

### 6. Квантификаторы и пагинация (`Any`, `All`, `Take`, `Skip`)
* Проверка наличия VIP-гостей (`Any`):
```csharp
bool hasVipGuests = db.Guests.Any(g => g.DiscountPercent >= 15);
```
* Проверка условия для всех номеров категории (`All`):
```csharp
bool allPresidentsHaveBalcony = db.Rooms
    .Where(r => r.RoomCategory!.Name == "Президентский люкс")
    .All(r => r.HasBalcony);
```
* Топ-3 самых дорогих броней (`Take(3)`):
```csharp
var top3Bookings = db.Bookings
    .OrderByDescending(b => b.TotalCost)
    .Take(3)
    .ToList();
```
* Постраничный вывод постояльцев (`Skip`, `Take`):
```csharp
int pageSize = 3;
int pageIndex = 1; // 2-я страница

var pagedGuests = db.Guests
    .OrderBy(g => g.Id)
    .Skip(pageIndex * pageSize)
    .Take(pageSize)
    .ToList();
```

---

## Начальные данные

В `DbInitializer.cs` создаются:
* 5 категорий номеров (Стандарт, Комфорт, Семейный, Полулюкс, Президентский);
* 10 номеров с 1 по 4 этажи;
* 7 гостей со скидками от 0 до 15%;
* 8 бронирований со всеми статусами;
* 11 дополнительных услуг (завтраки, трансфер, спа, паркинг, химчистка).

При каждом запуске база `hotel.db` пересоздаётся начисто (`EnsureDeleted` + `EnsureCreated`).

---

## Запуск проекта

### Через терминал:
```bash
cd UniversityLinqTemplate
dotnet run
```

### Через IDE (Visual Studio / Rider):
1. Открыть файл решения `UniversityLinqTemplate.slnx`.
2. Запустить проект (`Ctrl + F5`).
3. База данных `hotel.db` сформируется автоматически при старте.