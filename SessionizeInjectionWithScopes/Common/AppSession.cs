namespace SessionizeInjectionWithScopes.Common;

using System;

public sealed class AppSession : IDisposable
{
	private IServiceScope serviceScope;
	private static AppSession current;

	private AppSession(string user, IServiceScope serviceScope)
	{
		User = user;
		this.serviceScope = serviceScope;
	}

	public static AppSession Current 
	{
		get => current;
		private set => current = value; 
	}

	public string User { get; }

	public static void CreateSession(string user, IServiceProvider serviceProvider)
	{
		Current?.Dispose();
		Current = new AppSession(user, serviceProvider.CreateScope());
	}

	public TService Resolve<TService>() where TService : notnull
	{
		return serviceScope.ServiceProvider.GetRequiredService<TService>();
	}

	public void Dispose()
	{
		serviceScope?.Dispose();
		serviceScope = null;
		Current = null;
	}
}
