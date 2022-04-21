using NetMQ;
using NetMQ.Sockets;
using System.Collections.Specialized;

namespace TradePathCapital.Subscribers
{
	internal sealed class TpcSubscriber : IDisposable
	{
        private IList<ITpcListener> _listeners;
        private bool _disposed;
        private ITpcPublisherProperties _properties;
		internal TpcSubscriber(ITpcPublisherProperties properties)
		{
            _properties = properties;
            _listeners = new List<ITpcListener>();
            _disposed = false;
        }

		public void Dispose()
		{
            _disposed = true;
			foreach (ITpcListener listener in _listeners)
                try { listener.Disposed = true; listener.Socket.Dispose(); }
                finally { }
		}

        internal void Subscribe(string topicName)
		{
            var socket = new SubscriberSocket();
            socket.Options.ReceiveHighWatermark = 1000;
            socket.Connect(_properties.PublisherHost);
            socket.Subscribe(topicName);

            _listeners.Add(new TpcListener(new Thread(Listen), socket, topicName));
        }

        internal void Run()
		{
            foreach (ITpcListener listener in _listeners)
                listener.Thread.Start(listener);
		}

        private void Listen(object data)
		{
            var listener = data as ITpcListener;

            //Console.WriteLine($"Listening: {listener.TopicName}");
            using (listener.Socket)
            {
                while (!listener.Disposed)
                {
                    try
                    {
                        var topic = listener.Socket.ReceiveFrameString();
                        var message = listener.Socket.ReceiveFrameString();
                        Console.WriteLine(message);
                    }
                    catch (Exception ex)    
                    { Console.WriteLine(ex.Message); }
                }
            }
        }
    }
}
