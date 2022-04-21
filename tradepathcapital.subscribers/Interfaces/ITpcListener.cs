using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradePathCapital.Subscribers
{
	internal interface ITpcListener
	{
		Thread Thread { get; }
		SubscriberSocket Socket { get; }
		string TopicName { get; }
		bool Disposed { get; set; }

	}
}
