using NetMQ;
using NetMQ.Sockets;
using System.Configuration;

namespace tradepathcapital
{
	internal sealed class TpcDataManager : IDisposable
	{
		private NetMQRuntime? _runtime;
		private string _host;
		private List<TpcDataReader> _readers;
		private SortedDictionary<long, ITpcData> _data;

		public TpcDataManager()
		{
			_data = new SortedDictionary<long, ITpcData>();
			_readers = new List<TpcDataReader>();

			ReadConfig();

			_runtime = new NetMQRuntime();
		}

		public void Dispose()
		{
			if (_runtime != null)
				try { _runtime.Dispose(); } finally { _runtime = null; }
			foreach (TpcDataReader reader in _readers)
				try { reader.Dispose(); } finally { }
		}

		private void ReadConfig()
		{
			_host = ConfigurationManager.AppSettings["fileservice"];
			var path = ConfigurationManager.AppSettings["filespath"];
			if (!string.IsNullOrEmpty(path))
				try { path = Path.GetFullPath(path); }
				catch { throw new ConfigurationException("Incorrect path specified in app.config: 'filespath' key"); }

			int i = 1;
			while (true)
			{
				var filename = ConfigurationManager.AppSettings[$"file{i}"];
				
				if (string.IsNullOrEmpty(filename)) break;
				
				var filepath = Path.Join(path, filename);
				_readers.Add(new TpcDataReader(_host, filepath, i));
				i++;
			}

			if (_readers.Count == 0)
				throw new ConfigurationException("There is no files specified in app.config: 'fileN' key");
		}

		internal async Task ServerAsync()
		{
			using (var server = new RouterSocket(_host))
			{
				while (_readers.Select(x => x.Active).Count() > 0)
				{
					var (routingKey, more) = await server.ReceiveRoutingKeyAsync();
					var (message, _) = await server.ReceiveFrameStringAsync();

					TpcData item = TpcData.FromString(message);

					_data.Add(DateTime.UtcNow.Ticks, item);
				}
			}
		}

		public void Run()
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
