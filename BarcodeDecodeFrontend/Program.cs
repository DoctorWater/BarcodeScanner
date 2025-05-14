using System.Reflection;
using BarcodeDecodeFrontend.Data.Services;
using BarcodeDecodeFrontend.Data.Services.Auth;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using BarcodeDecodeFrontend.Data.Services.Messaging;
using BarcodeDecodeFrontend.Data.Services.Processing;
using BarcodeDecodeLib.Models.Dtos.Configs;
using BarcodeDecodeLib.Utils.Time;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;
using Serilog.Sinks.OpenSearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredModal()
    .AddBlazoredToast();
builder.Services.AddOptions<HttpAddresses>().Bind(builder.Configuration.GetSection(nameof(HttpAddresses)));
builder.Services.AddOptions<TimeZoneSettings>().Bind(builder.Configuration.GetSection(nameof(TimeZoneSettings)));

var addresses = builder.Configuration.GetRequiredSection(nameof(HttpAddresses)).Get<HttpAddresses>();
builder.Services.AddHttpClient<HttpMessagePublisher>((sp, client) =>
{
    client.BaseAddress = new Uri(addresses.BarcodeDecodeBackendAddress);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IHttpMessagePublisher, HttpMessagePublisher>();
builder.Services.AddScoped<IBarcodeDecoder, BarcodeDecoder>()
    .AddScoped<IVideoProcessor, VideoProcessor>()
    .AddScoped<ITimeConverter, TimeConverter>();

var url = builder.Configuration.GetRequiredSection("ApplicationUrl").Value!;
builder.WebHost.UseUrls(url);

#region Authorization
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddHttpClient("API", client =>
        client.BaseAddress = new Uri(addresses.BarcodeDecodeBackendAddress));
builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

builder.Host.UseSerilog((builderContext, cfg) =>
{
    cfg.ReadFrom.Configuration(builderContext.Configuration);
    Serilog.Debugging.SelfLog.Enable(Console.Out);

    var openSearchLogConfig = builder.Configuration.GetRequiredSection(nameof(ElasticSearchLogConfig))
        .Get<ElasticSearchLogConfig>()!;
    cfg.Enrich.FromLogContext();
    cfg.WriteTo.OpenSearch(new OpenSearchSinkOptions(openSearchLogConfig.Uri)
    {
        IndexFormat = openSearchLogConfig.IndexFormat,
        ModifyConnectionSettings = (c)
            => c.BasicAuthentication(openSearchLogConfig.Username, openSearchLogConfig.Password),
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
        AutoRegisterTemplate = openSearchLogConfig.AutoRegisterTemplate,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.OSv2,
        InlineFields = true
    });
});

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