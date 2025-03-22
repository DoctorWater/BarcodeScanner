using System.Reflection;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeDataAccess;
using BarcodeDecodeLib.Models.Dtos;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
var appName = Assembly.GetExecutingAssembly().GetName().Name ?? "<NO NAME>";
var builder = WebApplication.CreateBuilder(args);
var url = builder.Configuration.GetRequiredSection("ApplicationUrl").Value!;
builder.WebHost.UseUrls(url);

builder.Services.AddScoped<IBarcodeMessageHandler, BarcodeMessageHandler>();
var npgSqlConnection = AddDataAccess(builder, nameof(NpgSqlConfig));

builder.Services
    .AddDbContextPool<BarcodeDecodeDbContext>(opt =>
    {
        opt.UseNpgsql(npgSqlConnection, pgOpt => pgOpt.MigrationsAssembly("BarcodeDecodeDataAccess"))
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention();
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOptions<HttpAddresses>()
    .Bind(builder.Configuration.GetSection(nameof(HttpAddresses)));

var app = builder.Build();


app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();


string AddDataAccess(WebApplicationBuilder builder, string sectionName)
{
    var npgSqlConfig = builder.Configuration.GetRequiredSection(sectionName).Get<NpgSqlConfig>()!;
    var dbConnBuilder = new NpgsqlConnectionStringBuilder
    {
        Database = npgSqlConfig.Database,
        Host = npgSqlConfig.Host,
        Port = npgSqlConfig.Port,
        Username = npgSqlConfig.Username,
        Password = npgSqlConfig.Password,
        IncludeErrorDetail = npgSqlConfig.IncludeErrorDetail,
        Pooling = npgSqlConfig.Pooling,
        Enlist = npgSqlConfig.Enlist,
        CommandTimeout = npgSqlConfig.CommandTimeoutSeconds,
        Timeout = npgSqlConfig.TimeoutSeconds,
        KeepAlive = npgSqlConfig.KeepAliveSeconds,
        ApplicationName = appName,
        MaxPoolSize = 300,
    };

    return dbConnBuilder.ToString();
}