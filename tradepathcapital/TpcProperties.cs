using System.Configuration;

namespace TradePathCapital
{
	public class TpcProperties : ITpcDataManagerProperties, ITpcPublisherProperties
	{
		private const string PUBLISHER_HOST_KEY = "publisher_host";
		private const string MANAGER_HOST_KEY = "manager_host";
		private const string FILES_PREFIX = "file";

		private string _publisherHost;
		private string _managerHost;
		private string[] _files;

		public TpcProperties()
		{
			_publisherHost = ConfigurationManager.AppSettings[PUBLISHER_HOST_KEY]; ;

			_managerHost = ConfigurationManager.AppSettings[MANAGER_HOST_KEY];

			var files = new List<string>();
			while (true)
			{
				var filename = ConfigurationManager.AppSettings[$"{FILES_PREFIX}{files.Count}"];

				if (string.IsNullOrEmpty(filename)) break;

				try
				{
					Path.GetFullPath(filename);
					files.Add(filename);
				}
				catch
				{
					files = null;
					break;
				}
			}
			if (files != null)
				_files = files.ToArray();
		}

		public string PublisherHost
		{
			get
			{
				if (string.IsNullOrEmpty(_publisherHost))
					throw new ConfigurationErrorsException(
						$"No host specified for a publisher in app.config: '{PUBLISHER_HOST_KEY}' key");
				return _publisherHost;
			}
		}
		public string ManagerHost
		{
			get
			{
				if (string.IsNullOrEmpty(_managerHost))
					throw new ConfigurationErrorsException(
						$"No host specified for a manager in app.config: '{MANAGER_HOST_KEY}' key");
				return _managerHost;
			}
		}
		public string[] Files
		{
			get
			{
				if (_files == null || _files.Length == 0)
					throw new ConfigurationErrorsException(
						$"There is no files specified in app.config or at least one path value is incorrect or unacessable: '{FILES_PREFIX}N' key");
				return _files;
			}
		}
	}
}
