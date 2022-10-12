namespace SessionizeInjectionWithScopes;

using RxUI.MauiToolkit.Configuration;
using SessionizeInjectionWithScopes.Configuration;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
							 .InitRxToolkit()
							 .AddConfiguration();

		builder.Services.AddBootstrapper();

		return builder.Build();
	}
}
