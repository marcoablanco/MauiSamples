namespace PerformanceInit.App;

using Microsoft.Extensions.DependencyInjection;
using PerformanceInit.App.Services;
using PerformanceInit.App.Services.IoCServices;
using PerformanceInit.App.Services.SPServices;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
							 .UseMauiApp<App>()
							 .ConfigureFonts(fonts =>
							 {
								 fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
								 fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
							 });

		builder.Services.AddTransient(s => new MainPage(s));


		builder.Services.AddTransient<IServiceIoC1, ServiceIoC1>();
		builder.Services.AddTransient<IServiceIoC2, ServiceIoC2>();
		builder.Services.AddTransient<IServiceIoC3, ServiceIoC3>();
		builder.Services.AddTransient<IServiceIoC4, ServiceIoC4>();
		builder.Services.AddTransient<IServiceIoC5, ServiceIoC5>();
		//builder.Services.AddTransient<IServiceSP1, ServiceSP1>();
		//builder.Services.AddTransient<IServiceSP2, ServiceSP2>();
		//builder.Services.AddTransient<IServiceSP3, ServiceSP3>();
		//builder.Services.AddTransient<IServiceSP4, ServiceSP4>();
		//builder.Services.AddTransient<IServiceSP5, ServiceSP5>();


		//builder.Services.AddSingleton<IServiceIoC1, ServiceIoC1>();
		//builder.Services.AddSingleton<IServiceIoC2, ServiceIoC2>();
		//builder.Services.AddSingleton<IServiceIoC3, ServiceIoC3>();
		//builder.Services.AddSingleton<IServiceIoC4, ServiceIoC4>();
		//builder.Services.AddSingleton<IServiceIoC5, ServiceIoC5>();
		//builder.Services.AddSingleton<IServiceSP1, ServiceSP1>();
		//builder.Services.AddSingleton<IServiceSP2, ServiceSP2>();
		//builder.Services.AddSingleton<IServiceSP3, ServiceSP3>();
		//builder.Services.AddSingleton<IServiceSP4, ServiceSP4>();
		//builder.Services.AddSingleton<IServiceSP5, ServiceSP5>();


		//builder.Services.AddTransient<IServiceIoC1>(s => new ServiceIoC1(s.GetService<IServiceIoC2>(), s.GetService<IServiceIoC3>(), s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC2>(s => new ServiceIoC2(s.GetService<IServiceIoC3>(), s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC3>(s => new ServiceIoC3(s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC4>(s => new ServiceIoC4(s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC5>(_ => new ServiceIoC5());
		builder.Services.AddTransient<IServiceSP1>(s => new ServiceSP1(s));
		builder.Services.AddTransient<IServiceSP2>(s => new ServiceSP2(s));
		builder.Services.AddTransient<IServiceSP3>(s => new ServiceSP3(s));
		builder.Services.AddTransient<IServiceSP4>(s => new ServiceSP4(s));
		builder.Services.AddTransient<IServiceSP5>(_ => new ServiceSP5());

		return builder.Build();
	}
}
