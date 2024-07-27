using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using HardWareMonitorService;
using HardWareMonitorService.Service;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging.Configuration;

[DllImport("kernel32.dll")]
static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
var handle = GetConsoleWindow();
ShowWindow(handle, 0);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET Monitoring";
});

builder.Services.AddSingleton<HardwareService>();
builder.Services.AddSingleton<ApiService>();
builder.Services.AddSingleton<RegistryService>();
builder.Services.AddSingleton<HardwareMonitoringService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();