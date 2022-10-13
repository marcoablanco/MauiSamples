namespace PerformanceInit.App;

using Microsoft.Extensions.DependencyInjection;
using PerformanceInit.App.Services;
using PerformanceInit.App.Services.IoCServices;
using PerformanceInit.App.Services.SPServices;
using System;
using System.Diagnostics;

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

		// OK Transient
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

		// Ok singleton
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


		// KO Código horrible
		//builder.Services.AddTransient<IServiceIoC1>(s => new ServiceIoC1(s.GetService<IServiceIoC2>(), s.GetService<IServiceIoC3>(), s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC2>(s => new ServiceIoC2(s.GetService<IServiceIoC3>(), s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC3>(s => new ServiceIoC3(s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC4>(s => new ServiceIoC4(s.GetService<IServiceIoC5>()));
		//builder.Services.AddTransient<IServiceIoC5>(_ => new ServiceIoC5());
		// OK transient
		builder.Services.AddTransient<IServiceSP1>(s => new ServiceSP1(s));
		builder.Services.AddTransient<IServiceSP2>(s => new ServiceSP2(s));
		builder.Services.AddTransient<IServiceSP3>(s => new ServiceSP3(s));
		builder.Services.AddTransient<IServiceSP4>(s => new ServiceSP4(s));
		builder.Services.AddTransient<IServiceSP5>(_ => new ServiceSP5());

		// KO Código horrible
		//builder.Services.AddSingleton<IServiceIoC1>(s => new ServiceIoC1(s.GetService<IServiceIoC2>(), s.GetService<IServiceIoC3>(), s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddSingleton<IServiceIoC2>(s => new ServiceIoC2(s.GetService<IServiceIoC3>(), s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddSingleton<IServiceIoC3>(s => new ServiceIoC3(s.GetService<IServiceIoC4>(), s.GetService<IServiceIoC5>()));
		//builder.Services.AddSingleton<IServiceIoC4>(s => new ServiceIoC4(s.GetService<IServiceIoC5>()));
		//builder.Services.AddSingleton<IServiceIoC5>(_ => new ServiceIoC5());
		// OK transient
		//builder.Services.AddSingleton<IServiceSP1>(s => new ServiceSP1(s));
		//builder.Services.AddSingleton<IServiceSP2>(s => new ServiceSP2(s));
		//builder.Services.AddSingleton<IServiceSP3>(s => new ServiceSP3(s));
		//builder.Services.AddSingleton<IServiceSP4>(s => new ServiceSP4(s));
		//builder.Services.AddSingleton<IServiceSP5>(_ => new ServiceSP5());


		var stopwatch = Stopwatch.StartNew();
		//AddEmptyServicesAsSingleton(builder.Services);
		AddEmptyServicesAsTransient(builder.Services);

		stopwatch.Stop();
		Console.WriteLine($"---Tiempo en añadir los servicios: {stopwatch.ElapsedMilliseconds}.");
		return builder.Build();
	}

	private static void AddEmptyServicesAsTransient(IServiceCollection services)
	{
		services.AddTransient<IEmptyService000, EmptyService000>();
		services.AddTransient<IEmptyService001, EmptyService001>();
		services.AddTransient<IEmptyService002, EmptyService002>();
		services.AddTransient<IEmptyService003, EmptyService003>();
		services.AddTransient<IEmptyService004, EmptyService004>();
		services.AddTransient<IEmptyService005, EmptyService005>();
		services.AddTransient<IEmptyService006, EmptyService006>();
		services.AddTransient<IEmptyService007, EmptyService007>();
		services.AddTransient<IEmptyService008, EmptyService008>();
		services.AddTransient<IEmptyService009, EmptyService009>();
		services.AddTransient<IEmptyService010, EmptyService010>();
		services.AddTransient<IEmptyService011, EmptyService011>();
		services.AddTransient<IEmptyService012, EmptyService012>();
		services.AddTransient<IEmptyService013, EmptyService013>();
		services.AddTransient<IEmptyService014, EmptyService014>();
		services.AddTransient<IEmptyService015, EmptyService015>();
		services.AddTransient<IEmptyService016, EmptyService016>();
		services.AddTransient<IEmptyService017, EmptyService017>();
		services.AddTransient<IEmptyService018, EmptyService018>();
		services.AddTransient<IEmptyService019, EmptyService019>();
		services.AddTransient<IEmptyService020, EmptyService020>();
		services.AddTransient<IEmptyService021, EmptyService021>();
		services.AddTransient<IEmptyService022, EmptyService022>();
		services.AddTransient<IEmptyService023, EmptyService023>();
		services.AddTransient<IEmptyService024, EmptyService024>();
		services.AddTransient<IEmptyService025, EmptyService025>();
		services.AddTransient<IEmptyService026, EmptyService026>();
		services.AddTransient<IEmptyService027, EmptyService027>();
		services.AddTransient<IEmptyService028, EmptyService028>();
		services.AddTransient<IEmptyService029, EmptyService029>();
		services.AddTransient<IEmptyService030, EmptyService030>();
		services.AddTransient<IEmptyService031, EmptyService031>();
		services.AddTransient<IEmptyService032, EmptyService032>();
		services.AddTransient<IEmptyService033, EmptyService033>();
		services.AddTransient<IEmptyService034, EmptyService034>();
		services.AddTransient<IEmptyService035, EmptyService035>();
		services.AddTransient<IEmptyService036, EmptyService036>();
		services.AddTransient<IEmptyService037, EmptyService037>();
		services.AddTransient<IEmptyService038, EmptyService038>();
		services.AddTransient<IEmptyService039, EmptyService039>();
		services.AddTransient<IEmptyService040, EmptyService040>();
		services.AddTransient<IEmptyService041, EmptyService041>();
		services.AddTransient<IEmptyService042, EmptyService042>();
		services.AddTransient<IEmptyService043, EmptyService043>();
		services.AddTransient<IEmptyService044, EmptyService044>();
		services.AddTransient<IEmptyService045, EmptyService045>();
		services.AddTransient<IEmptyService046, EmptyService046>();
		services.AddTransient<IEmptyService047, EmptyService047>();
		services.AddTransient<IEmptyService048, EmptyService048>();
		services.AddTransient<IEmptyService049, EmptyService049>();
		services.AddTransient<IEmptyService050, EmptyService050>();
		services.AddTransient<IEmptyService051, EmptyService051>();
		services.AddTransient<IEmptyService052, EmptyService052>();
		services.AddTransient<IEmptyService053, EmptyService053>();
		services.AddTransient<IEmptyService054, EmptyService054>();
		services.AddTransient<IEmptyService055, EmptyService055>();
		services.AddTransient<IEmptyService056, EmptyService056>();
		services.AddTransient<IEmptyService057, EmptyService057>();
		services.AddTransient<IEmptyService058, EmptyService058>();
		services.AddTransient<IEmptyService059, EmptyService059>();
		services.AddTransient<IEmptyService060, EmptyService060>();
		services.AddTransient<IEmptyService061, EmptyService061>();
		services.AddTransient<IEmptyService062, EmptyService062>();
		services.AddTransient<IEmptyService063, EmptyService063>();
		services.AddTransient<IEmptyService064, EmptyService064>();
		services.AddTransient<IEmptyService065, EmptyService065>();
		services.AddTransient<IEmptyService066, EmptyService066>();
		services.AddTransient<IEmptyService067, EmptyService067>();
		services.AddTransient<IEmptyService068, EmptyService068>();
		services.AddTransient<IEmptyService069, EmptyService069>();
		services.AddTransient<IEmptyService070, EmptyService070>();
		services.AddTransient<IEmptyService071, EmptyService071>();
		services.AddTransient<IEmptyService072, EmptyService072>();
		services.AddTransient<IEmptyService073, EmptyService073>();
		services.AddTransient<IEmptyService074, EmptyService074>();
		services.AddTransient<IEmptyService075, EmptyService075>();
		services.AddTransient<IEmptyService076, EmptyService076>();
		services.AddTransient<IEmptyService077, EmptyService077>();
		services.AddTransient<IEmptyService078, EmptyService078>();
		services.AddTransient<IEmptyService079, EmptyService079>();
		services.AddTransient<IEmptyService080, EmptyService080>();
		services.AddTransient<IEmptyService081, EmptyService081>();
		services.AddTransient<IEmptyService082, EmptyService082>();
		services.AddTransient<IEmptyService083, EmptyService083>();
		services.AddTransient<IEmptyService084, EmptyService084>();
		services.AddTransient<IEmptyService085, EmptyService085>();
		services.AddTransient<IEmptyService086, EmptyService086>();
		services.AddTransient<IEmptyService087, EmptyService087>();
		services.AddTransient<IEmptyService088, EmptyService088>();
		services.AddTransient<IEmptyService089, EmptyService089>();
		services.AddTransient<IEmptyService090, EmptyService090>();
		services.AddTransient<IEmptyService091, EmptyService091>();
		services.AddTransient<IEmptyService092, EmptyService092>();
		services.AddTransient<IEmptyService093, EmptyService093>();
		services.AddTransient<IEmptyService094, EmptyService094>();
		services.AddTransient<IEmptyService095, EmptyService095>();
		services.AddTransient<IEmptyService096, EmptyService096>();
		services.AddTransient<IEmptyService097, EmptyService097>();
		services.AddTransient<IEmptyService098, EmptyService098>();
		services.AddTransient<IEmptyService099, EmptyService099>();
	}

	private static void AddEmptyServicesAsSingleton(IServiceCollection services)
	{
		services.AddSingleton<IEmptyService000, EmptyService000>();
		services.AddSingleton<IEmptyService001, EmptyService001>();
		services.AddSingleton<IEmptyService002, EmptyService002>();
		services.AddSingleton<IEmptyService003, EmptyService003>();
		services.AddSingleton<IEmptyService004, EmptyService004>();
		services.AddSingleton<IEmptyService005, EmptyService005>();
		services.AddSingleton<IEmptyService006, EmptyService006>();
		services.AddSingleton<IEmptyService007, EmptyService007>();
		services.AddSingleton<IEmptyService008, EmptyService008>();
		services.AddSingleton<IEmptyService009, EmptyService009>();
		services.AddSingleton<IEmptyService010, EmptyService010>();
		services.AddSingleton<IEmptyService011, EmptyService011>();
		services.AddSingleton<IEmptyService012, EmptyService012>();
		services.AddSingleton<IEmptyService013, EmptyService013>();
		services.AddSingleton<IEmptyService014, EmptyService014>();
		services.AddSingleton<IEmptyService015, EmptyService015>();
		services.AddSingleton<IEmptyService016, EmptyService016>();
		services.AddSingleton<IEmptyService017, EmptyService017>();
		services.AddSingleton<IEmptyService018, EmptyService018>();
		services.AddSingleton<IEmptyService019, EmptyService019>();
		services.AddSingleton<IEmptyService020, EmptyService020>();
		services.AddSingleton<IEmptyService021, EmptyService021>();
		services.AddSingleton<IEmptyService022, EmptyService022>();
		services.AddSingleton<IEmptyService023, EmptyService023>();
		services.AddSingleton<IEmptyService024, EmptyService024>();
		services.AddSingleton<IEmptyService025, EmptyService025>();
		services.AddSingleton<IEmptyService026, EmptyService026>();
		services.AddSingleton<IEmptyService027, EmptyService027>();
		services.AddSingleton<IEmptyService028, EmptyService028>();
		services.AddSingleton<IEmptyService029, EmptyService029>();
		services.AddSingleton<IEmptyService030, EmptyService030>();
		services.AddSingleton<IEmptyService031, EmptyService031>();
		services.AddSingleton<IEmptyService032, EmptyService032>();
		services.AddSingleton<IEmptyService033, EmptyService033>();
		services.AddSingleton<IEmptyService034, EmptyService034>();
		services.AddSingleton<IEmptyService035, EmptyService035>();
		services.AddSingleton<IEmptyService036, EmptyService036>();
		services.AddSingleton<IEmptyService037, EmptyService037>();
		services.AddSingleton<IEmptyService038, EmptyService038>();
		services.AddSingleton<IEmptyService039, EmptyService039>();
		services.AddSingleton<IEmptyService040, EmptyService040>();
		services.AddSingleton<IEmptyService041, EmptyService041>();
		services.AddSingleton<IEmptyService042, EmptyService042>();
		services.AddSingleton<IEmptyService043, EmptyService043>();
		services.AddSingleton<IEmptyService044, EmptyService044>();
		services.AddSingleton<IEmptyService045, EmptyService045>();
		services.AddSingleton<IEmptyService046, EmptyService046>();
		services.AddSingleton<IEmptyService047, EmptyService047>();
		services.AddSingleton<IEmptyService048, EmptyService048>();
		services.AddSingleton<IEmptyService049, EmptyService049>();
		services.AddSingleton<IEmptyService050, EmptyService050>();
		services.AddSingleton<IEmptyService051, EmptyService051>();
		services.AddSingleton<IEmptyService052, EmptyService052>();
		services.AddSingleton<IEmptyService053, EmptyService053>();
		services.AddSingleton<IEmptyService054, EmptyService054>();
		services.AddSingleton<IEmptyService055, EmptyService055>();
		services.AddSingleton<IEmptyService056, EmptyService056>();
		services.AddSingleton<IEmptyService057, EmptyService057>();
		services.AddSingleton<IEmptyService058, EmptyService058>();
		services.AddSingleton<IEmptyService059, EmptyService059>();
		services.AddSingleton<IEmptyService060, EmptyService060>();
		services.AddSingleton<IEmptyService061, EmptyService061>();
		services.AddSingleton<IEmptyService062, EmptyService062>();
		services.AddSingleton<IEmptyService063, EmptyService063>();
		services.AddSingleton<IEmptyService064, EmptyService064>();
		services.AddSingleton<IEmptyService065, EmptyService065>();
		services.AddSingleton<IEmptyService066, EmptyService066>();
		services.AddSingleton<IEmptyService067, EmptyService067>();
		services.AddSingleton<IEmptyService068, EmptyService068>();
		services.AddSingleton<IEmptyService069, EmptyService069>();
		services.AddSingleton<IEmptyService070, EmptyService070>();
		services.AddSingleton<IEmptyService071, EmptyService071>();
		services.AddSingleton<IEmptyService072, EmptyService072>();
		services.AddSingleton<IEmptyService073, EmptyService073>();
		services.AddSingleton<IEmptyService074, EmptyService074>();
		services.AddSingleton<IEmptyService075, EmptyService075>();
		services.AddSingleton<IEmptyService076, EmptyService076>();
		services.AddSingleton<IEmptyService077, EmptyService077>();
		services.AddSingleton<IEmptyService078, EmptyService078>();
		services.AddSingleton<IEmptyService079, EmptyService079>();
		services.AddSingleton<IEmptyService080, EmptyService080>();
		services.AddSingleton<IEmptyService081, EmptyService081>();
		services.AddSingleton<IEmptyService082, EmptyService082>();
		services.AddSingleton<IEmptyService083, EmptyService083>();
		services.AddSingleton<IEmptyService084, EmptyService084>();
		services.AddSingleton<IEmptyService085, EmptyService085>();
		services.AddSingleton<IEmptyService086, EmptyService086>();
		services.AddSingleton<IEmptyService087, EmptyService087>();
		services.AddSingleton<IEmptyService088, EmptyService088>();
		services.AddSingleton<IEmptyService089, EmptyService089>();
		services.AddSingleton<IEmptyService090, EmptyService090>();
		services.AddSingleton<IEmptyService091, EmptyService091>();
		services.AddSingleton<IEmptyService092, EmptyService092>();
		services.AddSingleton<IEmptyService093, EmptyService093>();
		services.AddSingleton<IEmptyService094, EmptyService094>();
		services.AddSingleton<IEmptyService095, EmptyService095>();
		services.AddSingleton<IEmptyService096, EmptyService096>();
		services.AddSingleton<IEmptyService097, EmptyService097>();
		services.AddSingleton<IEmptyService098, EmptyService098>();
		services.AddSingleton<IEmptyService099, EmptyService099>();
	}
}
