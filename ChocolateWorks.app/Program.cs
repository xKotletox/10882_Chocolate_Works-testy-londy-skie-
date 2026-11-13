using ChocolateWorks.Models;
using ChocolateWorks.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

// Rejestracja serwisów w kontenerze DI
builder.Services.AddSingleton<ICocoaPlantation, CocoaPlantation>();
builder.Services.AddSingleton<ISugarBeetFarm, SugarBeetFarm>();
builder.Services.AddSingleton<ICocoaProcessor, CocoaProcessor>();
builder.Services.AddSingleton<ISugarRefinery, SugarRefinery>();
builder.Services.AddSingleton<IDairy, Dairy>();
builder.Services.AddSingleton<ILabelPrinter, LabelPrinter>();
builder.Services.AddSingleton<IChocolateFactory, ChocolateFactory>();

using var host = builder.Build();

var factory = host.Services.GetRequiredService<IChocolateFactory>();

Console.WriteLine("=== Fabryka czekolady ChocolateWorks ===");
Console.WriteLine($"Użytkownik: {Environment.UserName}");
Console.WriteLine($"Katalog:    {Environment.CurrentDirectory}");
Console.WriteLine();

ChocolateOrder[] orders =
[
    new(ChocolateType.Milk, 100),
    new(ChocolateType.Dark, 200),
    new(ChocolateType.White, 1000) // niepoprawne - za ciężka
];

foreach (var order in orders)
{
    try
    {
        var bar = factory.ProduceBar(order);
        Console.WriteLine($">>> Gotowa tabliczka: {bar}");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($">>> Zamówienie odrzucone: {ex.Message}");
    }
    Console.WriteLine();
}