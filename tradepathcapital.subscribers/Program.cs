using TradePathCapital;
using TradePathCapital.Subscribers;


var topics = Environment.GetCommandLineArgs().Skip(1);

var properties = new TpcProperties() as ITpcPublisherProperties;

using (var subs = new TpcSubscriber(properties))
{
	foreach (var topicName in topics)
		subs.Subscribe(topicName);
#if DEBUG
	try
	{
		subs.Run();
		while (true)
		{
			Console.ReadLine();
			break;
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
#endif
}

