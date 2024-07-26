using System.Net;
using System.Text;
using System.Text.Json;
using HardWareMonitorService.Entity;
using HardWareMonitorService.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using Mono.Unix.Native;

namespace HardWareMonitorService.Service;

public class ApiService
{
    private const string ServerUri = "https://localhost:44312";
    private HubConnection hubConnection;
    private HardwareMonitoringService hardwareMonitoringService;
    private CancellationToken token;
    private CancellationTokenSource source;

    public ApiService(HardwareMonitoringService hardwareMonitoringService)
    {
        this.hardwareMonitoringService = hardwareMonitoringService;
    }


    public async Task ConnectHub(string userPcId)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44312/monitoring")
            .WithAutomaticReconnect(new CustomRetryPolicy())
            .Build();

        hubConnection.On("GetUserPcId", () =>
        {
            Console.WriteLine(hubConnection.ConnectionId);
            hubConnection.SendAsync("Init", userPcId);
        });

        hubConnection.On("GetUserRTInfo", async () =>
        {
            source = new CancellationTokenSource();
            token = source.Token;
            Console.WriteLine("Я токеен");
            Task.Run( async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Я отправляю данные");
                    await hubConnection.SendAsync("Monitoring", hardwareMonitoringService.GetPcRTInfo(),
                        cancellationToken: token);
                    await Task.Delay(1000, token);
                }
            });
        });

        hubConnection.On("SendingStop", () =>
        {
            Console.WriteLine("все я не отправляю мониторинг");
            source.Cancel();
        });

        await hubConnection.StartAsync();
    }

    public async Task<bool> IsUserExist(string userPcId)
    {
        bool success = false;

        HttpResponseMessage response = new HttpResponseMessage();
        while (!success)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(ServerUri + $"/hardware?userPcId={userPcId}"),
                Method = HttpMethod.Get,
            };
            try
            {
                response = await httpClient.SendAsync(httpRequestMessage);
                success = true;
                Console.WriteLine("Я подключился))");
            }
            catch
            {
                Console.WriteLine("Я не подключился((");
                await Task.Delay(5000);
            }
        }

        return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> CreateUser(UserPc userPc)
    {
        var json = JsonSerializer.Serialize(userPc);
        var jsonContent = new StringContent(content: json, Encoding.UTF8, "application/json");
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
        {
            RequestUri = new Uri(ServerUri + "/hardware"),
            Method = HttpMethod.Post,
            Content = jsonContent
        };

        var response = await httpClient.SendAsync(httpRequestMessage);
        return response.StatusCode == HttpStatusCode.OK;
    }
}