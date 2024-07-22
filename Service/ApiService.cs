using System.Net;
using System.Text;
using System.Text.Json;
using HardWareMonitorService.Entity;
using Microsoft.AspNetCore.SignalR.Client;

namespace HardWareMonitorService.Service;

public class ApiService
{
    private const string ServerUri = "https://localhost:44312";
    private HubConnection hubConnection;


    public async Task ConnectHub(string userPcId)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44312/monitoring")
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On("GetUserPcId", () =>
        {
            Console.WriteLine(hubConnection.ConnectionId);
            hubConnection.SendAsync("Init", userPcId);
        });

        await hubConnection.StartAsync();


    }

    public async Task<bool> IsUserExist(string userPcId)
    {
        var httpClient = new HttpClient();
        var httpRequestMessage = new HttpRequestMessage()
        {
            RequestUri = new Uri(ServerUri + $"/hardware?userPcId={userPcId}"),
            Method = HttpMethod.Get,
        };
        var response = await httpClient.SendAsync(httpRequestMessage);

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