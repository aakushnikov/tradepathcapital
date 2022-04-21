using System.Diagnostics;
using TradePathCapital;
using TradePathCapital.Publishers;

var properties = new TpcProperties();
#if DEBUG
try
{
#endif
	using (var manager = new TpcDataManager(
		properties as ITpcDataManagerProperties,
		properties as ITpcPublisherProperties))
		manager.Run();
#if DEBUG
}
catch (Exception ex)
{
	Debug.WriteLine(ex.Message);
}
#endif
