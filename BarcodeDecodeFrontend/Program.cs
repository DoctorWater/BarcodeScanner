using System.Reflection;
using BarcodeDecodeFrontend.Data.Services;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using BarcodeDecodeFrontend.Data.Services.Messaging;
using BarcodeDecodeFrontend.Data.Services.Processing;
using BarcodeDecodeLib;
using BarcodeDecodeLib.Models.Dtos;
using BarcodeDecodeLib.Models.Dtos.Configs;
using BarcodeDecodeLib.Utils.Time;
using Blazored.Modal;
using Blazored.Toast;
using MassTransit;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredModal()
    .AddBlazoredToast();
builder.Services.AddOptions<HttpAddresses>().Bind(builder.Configuration.GetSection(nameof(HttpAddresses)));
builder.Services.AddOptions<TimeZoneSettings>().Bind(builder.Configuration.GetSection(nameof(TimeZoneSettings)));
builder.Services.AddHttpClient<HttpMessagePublisher>((sp, client) =>
     {
         var addresses = sp.GetRequiredService<IOptions<HttpAddresses>>().Value;
         client.BaseAddress = new Uri(addresses.BarcodeDecodeBackendAddress);
         client.Timeout = TimeSpan.FromSeconds(30);
     });
     builder.Services.AddScoped<HttpMessagePublisher>();
builder.Services.AddScoped<IBarcodeDecoder, BarcodeDecoder>()
    .AddScoped<IVideoProcessor, VideoProcessor>()
    .AddScoped<ITimeConverter, TimeConverter>();

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