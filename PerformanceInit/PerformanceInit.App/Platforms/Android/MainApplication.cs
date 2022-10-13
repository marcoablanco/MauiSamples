using Android.App;
using Android.Runtime;
using System.Diagnostics;

namespace PerformanceInit.App;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp()
	{

		Stopwatch stopwatch = Stopwatch.StartNew();
		MauiApp app = MauiProgram.CreateMauiApp();
		stopwatch.Stop();
		Console.WriteLine($"---Tiempo en crear MauiApp: {stopwatch.ElapsedMilliseconds}.");
		return app;
	}
}
