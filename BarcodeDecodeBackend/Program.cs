using System.Reflection;
using System.Text;
using BarcodeDecodeAccessControl;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeDataAccess;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeDataAccess.Repositories;
using BarcodeDecodeLib.Models.Dtos.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using Serilog.Sinks.OpenSearch;

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

builder.Host.UseSerilog((builderContext, cfg) =>
{
    cfg.ReadFrom.Configuration(builderContext.Configuration);
    Serilog.Debugging.SelfLog.Enable(Console.Out);

    var openSearchLogConfig = builder.Configuration.GetRequiredSection(nameof(ElasticSearchLogConfig)).Get<ElasticSearchLogConfig>()!;
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

builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Введите токен в формате **Bearer ТутТокен**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
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

#region Identity

var jwt = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);
var identitySqlConnection = AddDataAccess(builder, "IdentitySqlConfig");
builder.Services
    .AddDbContextPool<UserDbContext>(opt =>
    {
        opt.UseNpgsql(identitySqlConnection, pgOpt => pgOpt.MigrationsAssembly("BarcodeDecodeAccessControl"))
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention();
    });

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

#endregion

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOptions<HttpAddresses>()
    .Bind(builder.Configuration.GetSection(nameof(HttpAddresses)));
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(nameof(JwtSettings)));

builder.Services.AddScoped<ITransportStorageUnitRepository, TransportStorageUnitRepository>()
    .AddScoped<ITransportOrderRepository, TransportOrderRepository>()
    .AddScoped<ITransportOrderMessageHandler, TransportOrderMessageHandler>()
    .AddScoped<IAuthMessageHandler, AuthMessageHandler>()
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentityDbInitializer.InitializeAsync(services);
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
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