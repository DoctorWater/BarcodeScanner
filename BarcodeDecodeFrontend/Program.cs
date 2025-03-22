using BarcodeDecodeFrontend.Data.Services;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using BarcodeDecodeFrontend.Data.Services.Processing;
using BarcodeDecodeLib;
using BarcodeDecodeLib.Models.Dtos;
using Blazored.Modal;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredModal();

builder.Services.AddScoped<BarcodeDecoder>()
    .AddScoped<VideoProcessor>();

builder.Services.AddScoped<IBarcodeDecoder, BarcodeDecoder>()
    .AddScoped<IVideoProcessor, VideoProcessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();