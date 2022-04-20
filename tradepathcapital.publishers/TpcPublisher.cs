using NetMQ;
using NetMQ.Sockets;
using System.Text.Json;

namespace TradePathCapital.Publishers
{
	internal sealed class TpcPublisher : IDisposable
	{
        private string _host;
        private PublisherSocket _socket;

        internal TpcPublisher(string host)
		{
            _host = host;

            _socket = new PublisherSocket();
            _socket.Options.SendHighWatermark = 1000;
            _socket.Bind(_host);
        }

		public void Dispose()
		{
			if (_socket != null)
                try { _socket.Close(); }
                finally { }
		}

        internal async Task Publish(ITpcData item)
		{
            var json = TpcData.SerializeToJson(item);
            _socket.SendMoreFrame($"Topic{item.Id}").SendFrame(json);
        }
    }
}
