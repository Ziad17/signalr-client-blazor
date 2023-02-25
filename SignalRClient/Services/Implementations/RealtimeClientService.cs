using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRClient.Services.Abstractions;

namespace SignalRClient.Services.Implementations
{
    public class RealtimeClientService : IRealtimeClientService
    {
        private HubConnection _hubConnection = null!;

        public IRealtimeClientService Build(string host, string userAccessToken, bool withAutomaticRetry = false)
        {
            var builder = new HubConnectionBuilder()
                .WithUrl(host, options =>
                {
                    if (!string.IsNullOrEmpty(userAccessToken))
                    {
                        options.AccessTokenProvider = () => Task.FromResult(userAccessToken)!;
                    }
                });

            if (withAutomaticRetry)
                builder = builder.WithAutomaticReconnect();

            _hubConnection = builder.Build();
            return this;
        }

        public async Task InvokeAsync(string methodName, object[] args)
        {
            var fixedLengthArray = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var isNumber = int.TryParse(args[i].ToString(), out var output);

                fixedLengthArray[i] = isNumber ? output : args[i];

            }

            switch (fixedLengthArray.Length)
            {
                case 0:
                    await _hubConnection.InvokeAsync(methodName);
                    return;
                case 1:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0]);
                    return;
                case 2:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0],
                        fixedLengthArray[1]);
                    return;
                case 3:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0],
                        fixedLengthArray[1], fixedLengthArray[2]);
                    return;
                case 4:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0],
                        fixedLengthArray[1], fixedLengthArray[2], fixedLengthArray[3]);
                    return;
                case 5:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0],
                        fixedLengthArray[1], fixedLengthArray[2], fixedLengthArray[3], fixedLengthArray[4]);
                    return;
                case 6:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0],
                        fixedLengthArray[1], fixedLengthArray[2], fixedLengthArray[3], fixedLengthArray[4], fixedLengthArray[5]);
                    return;
                case 7:
                    await _hubConnection.InvokeAsync(methodName, fixedLengthArray[0],
                        fixedLengthArray[1], fixedLengthArray[2], fixedLengthArray[3], fixedLengthArray[4], fixedLengthArray[5], fixedLengthArray[6]);
                    return;
            }
        }

        public void OnClosed(Func<Exception, Task> callback)
        {
            _hubConnection.Closed += callback;
        }

        public IRealtimeClientService On<T>(string methodName, Action<T> action)
        {
            Console.WriteLine($"Hooked to method => {methodName}");
            if (_hubConnection.State == HubConnectionState.Connected)
                throw new InvalidOperationException("You must call On method before calling Connect method");

            _hubConnection.On(methodName, action);
            return this;
        }



        public IRealtimeClientService On<T>(string methodName, Action<string, T> action)
        {
            Console.WriteLine($"Hooked to method => {methodName}");
            if (_hubConnection.State == HubConnectionState.Connected)
                throw new InvalidOperationException("You must call On method before calling Connect method");

            _hubConnection.On(methodName, action);
            return this;
        }

        public async Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
                return;
            Console.WriteLine($"Started Connection");
            await _hubConnection.StartAsync();
            if (_hubConnection.State == HubConnectionState.Disconnected)
                Console.WriteLine("error");

        }

        public async Task StopAsync()
        {
            await _hubConnection.StopAsync();
        }

        public HubConnection Connection() => _hubConnection;
    }
}
