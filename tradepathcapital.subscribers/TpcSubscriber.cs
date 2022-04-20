using NetMQ;
using NetMQ.Sockets;

namespace TradePathCapital.Subscribers
{
	internal sealed class TpcSubscriber : IDisposable
	{
        private List<SubscriberSocket> sockets;
        private bool _disposed;
		internal TpcSubscriber()
		{
            sockets = new List<SubscriberSocket>();
            _disposed = false;
        }

		public void Dispose()
		{
            _disposed = true;
			foreach (SubscriberSocket socket in sockets)
                try { socket.Dispose(); }
                finally { sockets.Clear(); }
		}

		internal async Task Subscribe(string host, string topic)
		{
            var socket = new SubscriberSocket();
            sockets.Add(socket);
            using (socket)
            {
                socket.Options.ReceiveHighWatermark = 1000;
                // "tcp://localhost:12345"
                socket.Connect(host);
                socket.Subscribe(topic);
                while (!_disposed)
                {
                    var messageTopicReceived = await socket.ReceiveFrameStringAsync();
                    if (!messageTopicReceived.Item2) continue;
                    var message = await socket.ReceiveFrameStringAsync();
                    if (message.Item1 == null) Task.Delay(100);
                    Console.WriteLine(message.Item1);
                }
            }
        }
    }
}
