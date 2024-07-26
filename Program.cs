using HardWareMonitorService;
using HardWareMonitorService.Service;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET MonitoringTest";
});

builder.Services.AddSingleton<HardwareService>();
builder.Services.AddSingleton<ApiService>();
builder.Services.AddSingleton<RegistryService>();
builder.Services.AddSingleton<HardwareMonitoringService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();