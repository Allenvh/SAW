using SAW.Portal.Models;
using SAW.Portal.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<SawOptions>(builder.Configuration.GetSection("SAW"));
builder.Services.AddSingleton<ConfigLoader>();
builder.Services.AddSingleton<IUserGroupResolver, MockGroupResolver>();
builder.Services.AddSingleton<IdentityMapper>();
builder.Services.AddSingleton<PolicyEngine>();
builder.Services.AddSingleton<SessionBroker>();
builder.Services.AddSingleton<AuditLogRedactor>();
builder.Services.AddHostedService<SessionCleanupWorker>();

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().WriteTo.File("logs/saw-.json", rollingInterval: RollingInterval.Day, formatter: new Serilog.Formatting.Json.JsonFormatter()).CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();
app.MapRazorPages();
app.Run();
