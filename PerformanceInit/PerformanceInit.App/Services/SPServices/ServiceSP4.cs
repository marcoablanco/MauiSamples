namespace PerformanceInit.App.Services.SPServices;
internal class ServiceSP4 : BaseService, IServiceSP4
{
	private readonly IServiceSP5 serviceSP5;

	public ServiceSP4(IServiceProvider serviceProvider)
	{
		Ensure.NotNull(serviceProvider);

		serviceSP5 = serviceProvider.GetRequiredService<IServiceSP5>();
		EnsureResolves();
	}

	private void EnsureResolves()
	{
		Ensure.NotNull(serviceSP5);
	}
}
