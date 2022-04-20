using System.Configuration;

namespace TradePathCapital
{
	public class TpcProperties
	{
		private const string PUBLISHER_HOST_KEY = "publisher_host";
		private const string MANAGER_HOST_KEY = "manager_host";
		private const string PATH = "filespath";
		private const string FILES_PREFIX = "file";

		private static TpcProperties _instance;

		public string PublisherHost { get; private set; }
		public string ManagerHost { get; private set; }
		public string[] Files { get; private set; }

		public static TpcProperties Config
		{
			get
			{
				if (_instance == null)
					_instance = new TpcProperties();
				return _instance;
			}
		}

		private TpcProperties()
		{
			var pHost = ConfigurationManager.AppSettings[PUBLISHER_HOST_KEY];
			if (string.IsNullOrEmpty(pHost))
				throw new ConfigurationErrorsException(
					$"No host specified for a publisher in app.config: '{PUBLISHER_HOST_KEY}' key");
			PublisherHost = pHost;

			var mHost = ConfigurationManager.AppSettings[MANAGER_HOST_KEY];
			if (string.IsNullOrEmpty(mHost))
				throw new ConfigurationErrorsException(
					$"No host specified for a manager in app.config: '{MANAGER_HOST_KEY}' key");
			ManagerHost = mHost;

			var path = ConfigurationManager.AppSettings[PATH];
			if (!string.IsNullOrEmpty(path))
				try { path = Path.GetFullPath(path); }
				catch { throw new ConfigurationErrorsException(
					$"Incorrect path specified in app.config: '{PATH}' key"); }

			var files = new List<string>();
			while (true)
			{
				var filename = ConfigurationManager.AppSettings[$"{FILES_PREFIX}{files.Count}"];

				if (string.IsNullOrEmpty(filename)) break;

				var filepath = Path.Join(path, filename);
				files.Add(filepath);
			}

			if (files.Count == 0)
				throw new ConfigurationErrorsException(
					$"There is no files specified in app.config: '{FILES_PREFIX}N' key");
			Files = files.ToArray();
		}
	}
}
