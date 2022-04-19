using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace tradepathcapital
{
	public class TpcData : ITpcData
	{
		private int _id;
		private string _content;
		const char SHARP = '#';

		public int Id { get => _id; }
		public string Content { get => _content; }

		public TpcData(int id, string content)
		{
			_id = id;
			_content = content;
		}

		public override string ToString()
		{
			return $"{_id}{SHARP}{_content}";
		}

		public static TpcData FromString(string data)
		{
			if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("data");
			var i = data.IndexOf(SHARP);
			if (i == -1) throw new ArgumentException("data");
			var id = Convert.ToInt32(data.Substring(0, i));
			var content = data.Substring(i + 1);
			return new TpcData(id, content);
		}


	}

	public interface ITpcData
	{
		int Id { get; }
		string Content { get; }
	}
}
