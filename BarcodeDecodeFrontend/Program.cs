using System.Reflection;
using BarcodeDecodeFrontend.Data.Services;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using BarcodeDecodeFrontend.Data.Services.Messaging;
using BarcodeDecodeFrontend.Data.Services.Processing;
using BarcodeDecodeLib;
using BarcodeDecodeLib.Models.Dtos;
using Blazored.Modal;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredModal();
builder.Services.AddOptions<HttpAddresses>().Bind(builder.Configuration.GetSection(nameof(HttpAddresses)));

builder.Services.AddScoped<IBarcodeDecoder, BarcodeDecoder>()
    .AddScoped<IVideoProcessor, VideoProcessor>()
    .AddScoped<BarcodeMessagePublisher>();

var url = builder.Configuration.GetRequiredSection("ApplicationUrl").Value!;
builder.WebHost.UseUrls(url);

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();