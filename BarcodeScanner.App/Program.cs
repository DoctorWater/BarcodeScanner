using BarcodeDecodeLib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BarcodeDecoder>();

var app = builder.Build();
var decoder = app.Services.GetRequiredService<BarcodeDecoder>();

//detector.DetectBarcode("C:\\Users\\malck\\Downloads\\Telegram Desktop\\photo_2024-12-11_21-22-26.jpg");

byte[] imageData = File.ReadAllBytes("C:\\Users\\malck\\Downloads\\Telegram Desktop\\photo_2025-01-10_18-15-01.jpg");
decoder.DecodeFromSerializedImage(imageData);
decoder.DecodeFromImage("C:\\Users\\malck\\Downloads\\Telegram Desktop\\photo_2025-01-10_18-15-01.jpg");


app.MapGet("/", () => "Hello World!");

app.Run();