using System.Diagnostics;
using TradePathCapital;
using TradePathCapital.Subscribers;


#if DEBUG
try
{
#endif
	var topics = Environment.GetCommandLineArgs().Skip(1);

	using (var subs = new TpcSubscriber())
		foreach (var topicName in topics)
			subs.Subscribe(TpcProperties.Config.PublisherHost, topicName);
#if DEBUG
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
}
#endif
