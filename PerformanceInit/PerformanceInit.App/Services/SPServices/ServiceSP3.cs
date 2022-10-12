namespace PerformanceInit.App.Services.SPServices;
internal class ServiceSP3 : BaseService, IServiceSP3
{
	private readonly IServiceSP4 serviceSP4;
	private readonly IServiceSP5 serviceSP5;

	public ServiceSP3(IServiceProvider serviceProvider)
	{
		Ensure.NotNull(serviceProvider);

		serviceSP4 = serviceProvider.GetRequiredService<IServiceSP4>();
		serviceSP5 = serviceProvider.GetRequiredService<IServiceSP5>();
		EnsureResolves();
	}

	private void EnsureResolves()
	{
		Ensure.NotNull(serviceSP4);
		Ensure.NotNull(serviceSP5);
	}
}
