using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradePathCapital.Subscribers
{
	internal class TpcListener : ITpcListener
	{
		private SubscriberSocket _socket;
		private string _topicName;
		private Thread _thread;

		public Thread Thread {  get => _thread; }
		public SubscriberSocket Socket { get => _socket; }
		public string TopicName { get => _topicName; }
		public bool Disposed { get; set; }

		public TpcListener(Thread thread, SubscriberSocket socket, string topic)
		{
			_socket = socket;
			_topicName = topic;
			_thread = thread;
			Disposed = false;
		}

	}
}
