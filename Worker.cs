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
                var userPcId = registryService.GetUserPcId() ?? hardwareService.GetUserPCId();
                var isUserExist = await apiService.IsUserExist(userPcId);
                

                if (isUserExist)
                {
                    Console.WriteLine("Я заглушка в подключению к хабу");
                    await apiService.ConnectHub(userPcId);
                    while (true)
                    {
                        
                    }
                }
                else
                {
                    var userPc = hardwareService.GetUserPc();
                    userPc.Id = userPcId;
                    var isCreated = await apiService.CreateUser(userPc);
                    registryService.CreateRegistry(userPcId);
                    if (isCreated)
                    {
                        //хаб коннектн
                    }
                    else
                    {
                        //Чето с ошибкой
                    }
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