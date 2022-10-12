namespace PerformanceInit.App.Services.SPServices;
internal class ServiceSP2 : BaseService, IServiceSP2
{
	private readonly IServiceSP3 serviceSP3;
	private readonly IServiceSP4 serviceSP4;
	private readonly IServiceSP5 serviceSP5;

	public ServiceSP2(IServiceProvider serviceProvider)
	{
		Ensure.NotNull(serviceProvider);

		serviceSP3 = serviceProvider.GetRequiredService<IServiceSP3>();
		serviceSP4 = serviceProvider.GetRequiredService<IServiceSP4>();
		serviceSP5 = serviceProvider.GetRequiredService<IServiceSP5>();
		EnsureResolves();
	}

	private void EnsureResolves()
	{
		Ensure.NotNull(serviceSP3);
		Ensure.NotNull(serviceSP4);
		Ensure.NotNull(serviceSP5);
	}
}
