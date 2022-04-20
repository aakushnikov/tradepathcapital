using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradePathCapital
{
	public sealed class TpcData : ITpcData
	{
		private int _id;
		private string[] _content;
		private DateTime _dateTime;

		public int Id { get => _id; }
		public string[] Content { get => _content; }
		public DateTime DateTime { get => _dateTime; }

		[JsonConstructor]
		public TpcData(int Id, DateTime DateTime, string[] Content)
		{
			// {"Id":1,"DateTime":"2022-04-14T04:00:00.017611","Content":["170.52","100","100","170.08","170.36","1","E","26","17",""]}
			_id = Id;
			_content = Content;
			_dateTime = DateTime;
		}

		public TpcData(int id, string nessage)
		{
			_id = id;
			
			var a = nessage.Split(',');
			
			DateTime.TryParse(a.First(), out _dateTime);
			
			_content = a.Skip(1).ToArray();
		}

		public static string SerializeToJson(ITpcData item)
		{
			return JsonSerializer.Serialize<TpcData>(item as TpcData);
		}

		public static ITpcData DeserializeFromJson(string json)
		{
			return JsonSerializer.Deserialize(json, typeof(TpcData)) as ITpcData;
		}
	}
}
