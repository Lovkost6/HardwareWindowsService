using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HardWareMonitorService.Service;

namespace HardWareMonitorService;

public class Worker(
    HardwareService hardwareService,
    ApiService apiService,
    RegistryService registryService,
    HardwareMonitoringService hardwareMonitoringService,
    ILogger<Worker> logger) : BackgroundService
{
    private const string PathConfig = "C:\\Monitoring\\config.txt";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //Если у нас нет в реестре мы никак это не фиксим
                var userPcId = registryService.GetUserPcId();
                if (userPcId == null)
                {
                    userPcId = hardwareService.GetUserPCId();
                    registryService.CreateRegistry(userPcId);
                }

                var isUserExist = await apiService.IsUserExist(userPcId);


                if (isUserExist)
                {
                    Console.WriteLine("Я заглушка в подключению к хабу");
                    await apiService.ConnectHub(userPcId);
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                else
                {
                    var userPc = hardwareService.GetUserPc();
                    userPc.Id = userPcId;
                    await apiService.CreateUser(userPc);
                    await apiService.ConnectHub(userPcId);
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException e)
        {
            logger.LogError(e.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Message}", ex.Message);
            Environment.Exit(1);
        }
    }
}