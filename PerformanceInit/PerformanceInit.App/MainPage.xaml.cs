namespace PerformanceInit.App;

using PerformanceInit.App.Services;
using PerformanceInit.App.Services.IoCServices;
using PerformanceInit.App.Services.SPServices;
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
			Debug.WriteLine($"ITERATION: {i}.");
		}

		stopwatch.Stop();

		if (stopwatch.ElapsedMilliseconds < minSP || minSP == 0)
			minSP = stopwatch.ElapsedMilliseconds;
		if (stopwatch.ElapsedMilliseconds > maxSP)
			maxSP = stopwatch.ElapsedMilliseconds;

		string line = $"Servicios IServiceSP resueltos en: {stopwatch.ElapsedMilliseconds}ms.";
		Debug.WriteLine(line);
		Dispatcher.Dispatch(() => items.Add(line));
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
			Debug.WriteLine($"ITERATION: {i}.");
		}

		stopwatch.Stop();

		if (stopwatch.ElapsedMilliseconds < minIoC || minIoC == 0)
			minIoC = stopwatch.ElapsedMilliseconds;
		if (stopwatch.ElapsedMilliseconds > maxIoC)
			maxIoC = stopwatch.ElapsedMilliseconds;

		string line = $"Servicios IServiceIoC resueltos en: {stopwatch.ElapsedMilliseconds}ms.";
		Debug.WriteLine(line);
		Dispatcher.Dispatch(() => items.Add(line));
		SetLabelsMinMax();
	}

	private void SetLabelsMinMax()
	{
		LblIoC.Text = $"IoC. Min: {minIoC}. Max{maxIoC}.";
		LblSP.Text = $"SP. Min: {minSP}. Max{maxSP}."; 
	}
}

