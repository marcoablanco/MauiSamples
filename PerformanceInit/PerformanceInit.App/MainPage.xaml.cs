namespace PerformanceInit.App;

using PerformanceInit.App.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

public partial class MainPage : ContentPage
{
	private readonly IServiceProvider serviceProvider;
	private readonly Stopwatch stopwatch;
	private ObservableCollection<string> items;
	private long minIoC;
	private long maxIoC;
	private long minSP;
	private long maxSP;


	public MainPage(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;

		InitializeComponent();

		BtnIoC.Command = new Command(() => BtnIoCCommandExecute());
		BtnSP.Command = new Command(() => BtnSPCommandExecute());
		BtnEmptyService.Command = new Command(() => BtnEmptyServiceCommandExecute());

		items = new ObservableCollection<string>();
		ListTime.ItemsSource = items;

		stopwatch = Stopwatch.StartNew();
		stopwatch.Stop();
	}

	private void BtnSPCommandExecute()
	{
		stopwatch.Restart();

		for (int i = 0; i < 10; i++)
		{
			serviceProvider.GetService<IServiceSP1>().WriteName();
			serviceProvider.GetService<IServiceSP2>().WriteName();
			serviceProvider.GetService<IServiceSP3>().WriteName();
			serviceProvider.GetService<IServiceSP4>().WriteName();
			serviceProvider.GetService<IServiceSP5>().WriteName();

			serviceProvider.GetRequiredService<IServiceSP1>().WriteName();
			serviceProvider.GetRequiredService<IServiceSP2>().WriteName();
			serviceProvider.GetRequiredService<IServiceSP3>().WriteName();
			serviceProvider.GetRequiredService<IServiceSP4>().WriteName();
			serviceProvider.GetRequiredService<IServiceSP5>().WriteName();
			Debug.WriteLine($"---ITERATION: {i}.");
		}

		stopwatch.Stop();

		if (stopwatch.ElapsedMilliseconds < minSP || minSP == 0)
			minSP = stopwatch.ElapsedMilliseconds;
		if (stopwatch.ElapsedMilliseconds > maxSP)
			maxSP = stopwatch.ElapsedMilliseconds;

		string line = $"---Servicios IServiceSP resueltos en: {stopwatch.ElapsedMilliseconds}ms.";
		Debug.WriteLine(line);
		Dispatcher.Dispatch(() => items.Insert(0,line));
		SetLabelsMinMax();
	}

	private void BtnIoCCommandExecute()
	{
		stopwatch.Restart();

		for (int i = 0; i < 10; i++)
		{
			serviceProvider.GetService<IServiceIoC1>().WriteName();
			serviceProvider.GetService<IServiceIoC2>().WriteName();
			serviceProvider.GetService<IServiceIoC3>().WriteName();
			serviceProvider.GetService<IServiceIoC4>().WriteName();
			serviceProvider.GetService<IServiceIoC5>().WriteName();

			serviceProvider.GetRequiredService<IServiceIoC1>().WriteName();
			serviceProvider.GetRequiredService<IServiceIoC2>().WriteName();
			serviceProvider.GetRequiredService<IServiceIoC3>().WriteName();
			serviceProvider.GetRequiredService<IServiceIoC4>().WriteName();
			serviceProvider.GetRequiredService<IServiceIoC5>().WriteName();
			Debug.WriteLine($"---ITERATION: {i}.");
		}

		stopwatch.Stop();

		if (stopwatch.ElapsedMilliseconds < minIoC || minIoC == 0)
			minIoC = stopwatch.ElapsedMilliseconds;
		if (stopwatch.ElapsedMilliseconds > maxIoC)
			maxIoC = stopwatch.ElapsedMilliseconds;

		string line = $"---Servicios IServiceIoC resueltos en: {stopwatch.ElapsedMilliseconds}ms.";
		Debug.WriteLine(line);
		Dispatcher.Dispatch(() => items.Insert(0, line));
		SetLabelsMinMax(); 
	}

	private void SetLabelsMinMax()
	{
		LblIoC.Text = $"---IoC. Min: {minIoC}. Max{maxIoC}.";
		LblSP.Text = $"---SP. Min: {minSP}. Max{maxSP}.";
	}

	private void BtnEmptyServiceCommandExecute()
	{
		stopwatch.Restart();
		List<IBaseService> list = new List<IBaseService>
		{
			serviceProvider.GetRequiredService<IEmptyService000>(),
			serviceProvider.GetRequiredService<IEmptyService001>(),
			serviceProvider.GetRequiredService<IEmptyService002>(),
			serviceProvider.GetRequiredService<IEmptyService003>(),
			serviceProvider.GetRequiredService<IEmptyService004>(),
			serviceProvider.GetRequiredService<IEmptyService005>(),
			serviceProvider.GetRequiredService<IEmptyService006>(),
			serviceProvider.GetRequiredService<IEmptyService007>(),
			serviceProvider.GetRequiredService<IEmptyService008>(),
			serviceProvider.GetRequiredService<IEmptyService009>(),
			serviceProvider.GetRequiredService<IEmptyService010>(),
			serviceProvider.GetRequiredService<IEmptyService011>(),
			serviceProvider.GetRequiredService<IEmptyService012>(),
			serviceProvider.GetRequiredService<IEmptyService013>(),
			serviceProvider.GetRequiredService<IEmptyService014>(),
			serviceProvider.GetRequiredService<IEmptyService015>(),
			serviceProvider.GetRequiredService<IEmptyService016>(),
			serviceProvider.GetRequiredService<IEmptyService017>(),
			serviceProvider.GetRequiredService<IEmptyService018>(),
			serviceProvider.GetRequiredService<IEmptyService019>(),
			serviceProvider.GetRequiredService<IEmptyService020>(),
			serviceProvider.GetRequiredService<IEmptyService021>(),
			serviceProvider.GetRequiredService<IEmptyService022>(),
			serviceProvider.GetRequiredService<IEmptyService023>(),
			serviceProvider.GetRequiredService<IEmptyService024>(),
			serviceProvider.GetRequiredService<IEmptyService025>(),
			serviceProvider.GetRequiredService<IEmptyService026>(),
			serviceProvider.GetRequiredService<IEmptyService027>(),
			serviceProvider.GetRequiredService<IEmptyService028>(),
			serviceProvider.GetRequiredService<IEmptyService029>(),
			serviceProvider.GetRequiredService<IEmptyService030>(),
			serviceProvider.GetRequiredService<IEmptyService031>(),
			serviceProvider.GetRequiredService<IEmptyService032>(),
			serviceProvider.GetRequiredService<IEmptyService033>(),
			serviceProvider.GetRequiredService<IEmptyService034>(),
			serviceProvider.GetRequiredService<IEmptyService035>(),
			serviceProvider.GetRequiredService<IEmptyService036>(),
			serviceProvider.GetRequiredService<IEmptyService037>(),
			serviceProvider.GetRequiredService<IEmptyService038>(),
			serviceProvider.GetRequiredService<IEmptyService039>(),
			serviceProvider.GetRequiredService<IEmptyService040>(),
			serviceProvider.GetRequiredService<IEmptyService041>(),
			serviceProvider.GetRequiredService<IEmptyService042>(),
			serviceProvider.GetRequiredService<IEmptyService043>(),
			serviceProvider.GetRequiredService<IEmptyService044>(),
			serviceProvider.GetRequiredService<IEmptyService045>(),
			serviceProvider.GetRequiredService<IEmptyService046>(),
			serviceProvider.GetRequiredService<IEmptyService047>(),
			serviceProvider.GetRequiredService<IEmptyService048>(),
			serviceProvider.GetRequiredService<IEmptyService049>(),
			serviceProvider.GetRequiredService<IEmptyService050>(),
			serviceProvider.GetRequiredService<IEmptyService051>(),
			serviceProvider.GetRequiredService<IEmptyService052>(),
			serviceProvider.GetRequiredService<IEmptyService053>(),
			serviceProvider.GetRequiredService<IEmptyService054>(),
			serviceProvider.GetRequiredService<IEmptyService055>(),
			serviceProvider.GetRequiredService<IEmptyService056>(),
			serviceProvider.GetRequiredService<IEmptyService057>(),
			serviceProvider.GetRequiredService<IEmptyService058>(),
			serviceProvider.GetRequiredService<IEmptyService059>(),
			serviceProvider.GetRequiredService<IEmptyService060>(),
			serviceProvider.GetRequiredService<IEmptyService061>(),
			serviceProvider.GetRequiredService<IEmptyService062>(),
			serviceProvider.GetRequiredService<IEmptyService063>(),
			serviceProvider.GetRequiredService<IEmptyService064>(),
			serviceProvider.GetRequiredService<IEmptyService065>(),
			serviceProvider.GetRequiredService<IEmptyService066>(),
			serviceProvider.GetRequiredService<IEmptyService067>(),
			serviceProvider.GetRequiredService<IEmptyService068>(),
			serviceProvider.GetRequiredService<IEmptyService069>(),
			serviceProvider.GetRequiredService<IEmptyService070>(),
			serviceProvider.GetRequiredService<IEmptyService071>(),
			serviceProvider.GetRequiredService<IEmptyService072>(),
			serviceProvider.GetRequiredService<IEmptyService073>(),
			serviceProvider.GetRequiredService<IEmptyService074>(),
			serviceProvider.GetRequiredService<IEmptyService075>(),
			serviceProvider.GetRequiredService<IEmptyService076>(),
			serviceProvider.GetRequiredService<IEmptyService077>(),
			serviceProvider.GetRequiredService<IEmptyService078>(),
			serviceProvider.GetRequiredService<IEmptyService079>(),
			serviceProvider.GetRequiredService<IEmptyService080>(),
			serviceProvider.GetRequiredService<IEmptyService081>(),
			serviceProvider.GetRequiredService<IEmptyService082>(),
			serviceProvider.GetRequiredService<IEmptyService083>(),
			serviceProvider.GetRequiredService<IEmptyService084>(),
			serviceProvider.GetRequiredService<IEmptyService085>(),
			serviceProvider.GetRequiredService<IEmptyService086>(),
			serviceProvider.GetRequiredService<IEmptyService087>(),
			serviceProvider.GetRequiredService<IEmptyService088>(),
			serviceProvider.GetRequiredService<IEmptyService089>(),
			serviceProvider.GetRequiredService<IEmptyService090>(),
			serviceProvider.GetRequiredService<IEmptyService091>(),
			serviceProvider.GetRequiredService<IEmptyService092>(),
			serviceProvider.GetRequiredService<IEmptyService093>(),
			serviceProvider.GetRequiredService<IEmptyService094>(),
			serviceProvider.GetRequiredService<IEmptyService095>(),
			serviceProvider.GetRequiredService<IEmptyService096>(),
			serviceProvider.GetRequiredService<IEmptyService097>(),
			serviceProvider.GetRequiredService<IEmptyService098>(),
			serviceProvider.GetRequiredService<IEmptyService099>(),
		};
		stopwatch.Stop();

		string line = $"---Servicios vacios resueltos en: {stopwatch.ElapsedMilliseconds}ms.";
		Debug.WriteLine(line);
		Dispatcher.Dispatch(() => items.Insert(0, line));

		foreach (IBaseService service in list)
			Ensure.NotNull(service);
	}
}

