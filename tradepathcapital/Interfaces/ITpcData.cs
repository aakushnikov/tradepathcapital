namespace TradePathCapital
{
	public interface ITpcData
	{
		int Id { get; }
		DateTime DateTime { get; }
		string[] Content { get; }
	}
}
