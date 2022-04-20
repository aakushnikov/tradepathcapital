using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;
using System.Text.Json;

namespace TradePathCapital.Publishers
{
	internal sealed class TpcDataManager : IDisposable
	{
		private NetMQRuntime _runtime;
		private List<TpcDataReader> _readers;
		private SortedDictionary<DateTime, IList<ITpcData>> _data;
		private TpcPublisher _publisher;

		internal TpcDataManager()
		{
			_data = new SortedDictionary<DateTime, IList<ITpcData>>();
			_readers = new List<TpcDataReader>();

			for (int i = 0; i < TpcProperties.Config.Files.Length; i++)
				_readers.Add(new TpcDataReader(
					TpcProperties.Config.ManagerHost,
					TpcProperties.Config.Files[i],
					i));
			
			_publisher = new TpcPublisher(TpcProperties.Config.PublisherHost);

			_runtime = new NetMQRuntime();
		}

		public void Dispose()
		{
			if (_runtime != null)
				try { _runtime.Dispose(); }
				finally { }
			foreach (TpcDataReader reader in _readers)
				try { reader.Dispose(); }
				finally { }
			if (_publisher != null)
				try { _publisher.Dispose(); }
				finally { }
		}


		internal async Task ServerAsync()
		{
			using (var server = new RouterSocket(TpcProperties.Config.ManagerHost))
			{
				while (_readers.Select(x => x.Active).Count() > 0)
				{
					var (routingKey, more) = await server.ReceiveRoutingKeyAsync();
					var (message, _) = await server.ReceiveFrameStringAsync();
#if DEBUG
					try
					{
#endif
						var item = TpcData.DeserializeFromJson(message);
						if (_data.ContainsKey(item.DateTime))
							_data[item.DateTime].Add(item);
						else
							_data.Add(item.DateTime, new List<ITpcData> { item });

						_publisher.Publish(item);
#if DEBUG
					}
					catch (Exception ex)
					{
						Debug.WriteLine(message);
						Debug.WriteLine(ex.Message);
					}
#endif
				}
			}
		}

		internal void Run()
		{
			// TODO Donno how to run it dynamic as a delegates
			_runtime.Run(ServerAsync(),
				_readers[0].ReadAndSendAsync(),
				_readers[1].ReadAndSendAsync(),
				_readers[2].ReadAndSendAsync(),
				_readers[3].ReadAndSendAsync(),
				_readers[4].ReadAndSendAsync()
				);
		}
	}
}
