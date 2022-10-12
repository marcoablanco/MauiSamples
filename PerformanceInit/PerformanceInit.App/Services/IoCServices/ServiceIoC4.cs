namespace PerformanceInit.App.Services.IoCServices;
using Microsoft.Extensions.DependencyInjection;

internal class ServiceIoC4: BaseService, IServiceIoC4
{
	private readonly IServiceIoC5 serviceIoC5;

	public ServiceIoC4(IServiceIoC5 serviceIoC5)
	{
		Ensure.NotNull(serviceIoC5);

		this.serviceIoC5 = serviceIoC5;
		EnsureResolves();
	}

	private void EnsureResolves()
	{
		Ensure.NotNull(serviceIoC5);
	}
}
