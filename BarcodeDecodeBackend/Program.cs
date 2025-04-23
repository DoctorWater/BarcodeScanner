using System.Reflection;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeDataAccess;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeDataAccess.Repositories;
using BarcodeDecodeLib.Models.Dtos.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BarcodeDecodeBackend API",
        Version = "v1",
        Description = "Платформа для тестирования API бэкенда проекта BarcodeDecode"
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOptions<HttpAddresses>()
    .Bind(builder.Configuration.GetSection(nameof(HttpAddresses)));
builder.Services.AddScoped<ITransportStorageUnitRepository, TransportStorageUnitRepository>()
    .AddScoped<ITransportOrderRepository, TransportOrderRepository>()
    .AddScoped<ITransportOrderMessageHandler, TransportOrderMessageHandler>()
    .AddScoped<ITsuMessageHandler, TsuMessageHandler>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

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