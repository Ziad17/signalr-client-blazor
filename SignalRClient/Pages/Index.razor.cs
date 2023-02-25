using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SignalRClient.Extensions;
using SignalRClient.Services.Abstractions;
using SignalRClient.Shared.Components;
using SignalRClient.ViewModels;

namespace SignalRClient.Pages
{
    public partial class Index
    {
        private ConsoleComponent _consoleComponent;

        private bool _isExpanded = true;

        private bool _connected = false;

        public string Host { get; set; }

        public string InvokedMethod { get; set; }

        public string Parameters { get; set; }

        private Action<object> AttachLogsToConsole;

        private Func<Exception, Task> OnCloseCallBack;

        public string AuthKey { get; set; }

        public string Actions { get; set; }

        public int LogsLimit { get; set; } = 50;

        public bool BeautifyJson { get; set; } = false;

        public bool AutomaticRetry { get; set; } = true;

        [Inject] public IRealtimeClientService RealtimeClientService { get; set; }

        [Inject] public ISnackbar Snackbar { get; set; }

        public void ClearLogs()
        {
            _consoleComponent.Clear();
        }

        protected override void OnInitialized()
        {
            AttachLogsToConsole = (result) =>
            {
                var model = new SignalViewModel()
                {
                    Payload = result.ToString(),
                    DateTime = DateTime.UtcNow
                };

                Console.WriteLine(result);

                _consoleComponent.AddSignal(model);
                StateHasChanged();
            };

            OnCloseCallBack = exception =>
            {
                _connected = false;
                _isExpanded = true;
                if (exception != null)
                {
                    Snackbar.Add(exception.Message, Severity.Error);

                }
                return Task.CompletedTask;
            };
            base.OnInitialized();
        }

        public void ApplyFilters()
        {
            _consoleComponent.ApplyLimit(LogsLimit);
            //_consoleComponent.BeautifyJson = BeautifyJson;
        }

        public async Task ConnectAsync()
        {
            try
            {
                RealtimeClientService.Build(Host, AuthKey, true);

                var actionsList = Actions.ToListFromCommaSeparateString();

                foreach (var action in actionsList)
                {
                    RealtimeClientService.On(action, AttachLogsToConsole);
                }

                await RealtimeClientService.StartAsync();

                RealtimeClientService.OnClosed(OnCloseCallBack);

                _isExpanded = false;
                _connected = true;

                Snackbar.Add("Connected Successfully", Severity.Success);

            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }

        }

        public async Task InvokeMethodAsync()
        {
            try
            {
                var args = Parameters.ToListFromCommaSeparateString().ToArray();
                await RealtimeClientService.InvokeAsync(InvokedMethod, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Snackbar.Add(e.Message, Severity.Error);
            }

        }

        public async Task DisconnectAsync()
        {
            _connected = false;
            _isExpanded = false;
            await RealtimeClientService.StopAsync();

            Snackbar.Add("Manually Disconnected Successfully", Severity.Success);

        }


    }
}