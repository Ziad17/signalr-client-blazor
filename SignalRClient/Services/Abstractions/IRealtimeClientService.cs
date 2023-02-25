using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClient.Services.Abstractions
{
    public interface IRealtimeClientService
    {
        Task StartAsync();

        Task StopAsync();

        void OnClosed(Func<Exception, Task> callback);

        Task InvokeAsync(string methodName, object[] args);

        IRealtimeClientService On<T>(string methodName, Action<T> action);

        IRealtimeClientService On<T>(string methodName, Action<string, T> action);

        IRealtimeClientService Build(string host, string userAccessToken, bool withAutomaticRetry = false);

        HubConnection Connection();
    }
}