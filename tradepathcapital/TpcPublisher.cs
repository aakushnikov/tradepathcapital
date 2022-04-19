using NetMQ;
using NetMQ.Sockets;

namespace tradepathcapital
{
	internal sealed class TpcPublisher : IDisposable
	{
        PublisherSocket? _socket;
        internal TpcPublisher()
		{
            _socket = new PublisherSocket();
            _socket.Options.SendHighWatermark = 1000;
                //TODO move to config
            _socket.Bind("tcp://*:12345");
        }

		public void Dispose()
		{
			if (_socket != null)
                try { _socket.Close(); } finally { _socket = null; }
		}

		internal void Publish(ITpcData data)
		{
            _socket.SendMoreFrame("TopicA").SendFrame(data.Content);
            Thread.Sleep(500);

        }

        internal void Subscribe(string topic, int id)
		{

		}
    }
}
