using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace EventConsumer
{
    public class SignalRClient<T>
    {
        private HubConnection _hubConnection;
        private string _signalRUrl;
        private string _signalREventName;
        private T _receivedMessage;

        public SignalRClient(string signalRUrl)
        {
            _signalRUrl = signalRUrl;
        }
        public async Task StartAsync()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_signalRUrl)
                    .Build();
                _hubConnection.On<T>(_signalREventName, (message) =>
                {
                    _receivedMessage = message;
                });
                await _hubConnection.StartAsync();
            }
            catch(Exception ex) 
            {
                Log.Error($"problem with hup connection{ex}");
            }
        }
        public async Task<T> GetReceivedMessageAsync()
        {
            return _receivedMessage;
        }
        public async Task StopAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}