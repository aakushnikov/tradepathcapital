using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;

namespace tradepathcapital
{
	internal sealed class TpcDataReader : IDisposable
	{
		FileStream? _stream;
		StreamReader? _reader;
		string _path;
		string _host;

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
				try { _reader.Close(); } finally { _reader = null; }
			if (_stream != null)
				try { _stream.Close(); } finally { _stream = null; }

		}

		internal async Task ReadAndSendAsync()
		{
			using (var client = new DealerSocket(_host))
			{
				using (_stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
				using (_reader = new StreamReader(_stream))
				{
					//int i = 1;
					string s = _reader.ReadLine();
					while ((s = await _reader.ReadLineAsync()) != string.Empty)
					{
						client.SendFrame($"{Id}#{s}");
						//i++;
						//if (i % 1000 == 0)
						//	Debug.WriteLine($"{ID}: {i}");
						// TODO How to walk around this delay stuff? It's realy slow down speed
						await Task.Delay(1);
					}
				}
			}
			Dispose();
		}
	}
}
