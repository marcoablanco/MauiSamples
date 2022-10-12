namespace SessionizeInjectionWithScopes.Configuration;

using SessionizeInjectionWithScopes.Features.Login;
using SessionizeInjectionWithScopes.Features.Main;
using SessionizeInjectionWithScopes.Services.Authentication;
using SessionizeInjectionWithScopes.Services.NavigationPage;

internal static class AppBootstraper
{
	public static IServiceCollection AddBootstrapper(this IServiceCollection services)
	{
		return services.AddServices()
					   .AddViewModels()
					   .AddViews();
	}

	private static IServiceCollection AddServices(this IServiceCollection services)
	{
		return services.AddSingleton<IAuthenticationService>(_ => new MockedAuthenticationService());
	}

	private static IServiceCollection AddViews(this IServiceCollection services)
	{
		return services.AddScoped<INavigationPageService>(_ => new MainShell())
					   .AddTransient(s => new LoginPage(s));
	}

	private static IServiceCollection AddViewModels(this IServiceCollection services)
	{
		return services.AddTransient(s => new LoginViewModel(s));
	}
}
