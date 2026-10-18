using System;
using UniversityLinqTemplate.Data;
using UniversityLinqTemplate.Queries;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Лабораторная работа: LINQ");
Console.WriteLine("Тема: Гостиничный комплекс 'GrandHotel'\n");

using (var db = new UniversityContext())
{
    Console.WriteLine("Инициализация бд");
    DbInitializer.Initialize(db);
    Console.WriteLine("База готова.\n");

    LinqDemos.DemoFilteringAndSorting(db);
    LinqDemos.DemoProjection(db);
    LinqDemos.DemoAggregations(db);
    LinqDemos.DemoGrouping(db);
    LinqDemos.DemoJoinsAndIncludes(db);
    LinqDemos.DemoQuantifiersAndPaging(db);
}

Console.WriteLine("\nВсе запросы выполнены");

if (!Console.IsInputRedirected)
{
    Console.WriteLine("Нажмите любую клавишу для выхода...");
    Console.ReadKey();
}