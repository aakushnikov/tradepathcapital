using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;

namespace TradePathCapital.Publishers
{
	internal sealed class TpcDataReader : IDisposable
	{
		private FileStream _stream;
		private StreamReader _reader;
		private string _path;
		private string _host;

		public int Id { get; private set; }
		public bool Active { get; private set; }

		internal TpcDataReader(string host, string path, int id)
		{
			_path = path;
			_host = host;
			Id = id;
			Active = true;
		}

		public void Dispose()
		{
			Active = false;

			if (_reader != null)
				try { _reader.Close(); }
				finally { }
			if (_stream != null)
				try { _stream.Close(); }
				finally { }

		}

		internal async Task ReadAndSendAsync()
		{
			using (var client = new DealerSocket(_host))
			{

				using (_stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
				using (_reader = new StreamReader(_stream))
				{
#if DEBUG
					int i = 1;
#endif
					string s = _reader.ReadLine();
					while ((s = await _reader.ReadLineAsync()) != string.Empty)
					{
#if DEBUG
						try
						{
#endif
							var item = new TpcData(Id, s);
							var json = TpcData.SerializeToJson(item);
							client.SendFrame(json);
#if DEBUG
						}
						catch (Exception ex)
						{
							Debug.WriteLine(s);
							Debug.WriteLine(ex.Message);
						}
						i++;
						if (i % 1000 == 0)
							Debug.WriteLine($"{Id}: {i}");
#endif
						// TODO How to walk around this delay stuff? It's realy slow down speed
						await Task.Delay(1);
					}
				}
			}
			Dispose();
		}
	}
}
