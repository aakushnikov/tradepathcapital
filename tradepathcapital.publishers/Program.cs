using System.Diagnostics;
using TradePathCapital.Publishers;

#if DEBUG
try
{
#endif
	using (var manager = new TpcDataManager())
		manager.Run();



#if DEBUG
}
catch (Exception ex)
{
	Debug.WriteLine(ex.Message);
}
#endif
