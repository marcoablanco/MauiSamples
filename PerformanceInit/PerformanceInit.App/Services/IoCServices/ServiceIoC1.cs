namespace PerformanceInit.App.Services.IoCServices;

using System;

internal class ServiceIoC1 : BaseService, IServiceIoC1
{
	private readonly IServiceIoC2 serviceIoC2;
	private readonly IServiceIoC3 serviceIoC3;
	private readonly IServiceIoC4 serviceIoC4;
	private readonly IServiceIoC5 serviceIoC5;

	public ServiceIoC1(IServiceIoC2 serviceIoC2, IServiceIoC3 serviceIoC3, IServiceIoC4 serviceIoC4, IServiceIoC5 serviceIoC5)
	{
		Ensure.NotNull(serviceIoC2);
		Ensure.NotNull(serviceIoC3);
		Ensure.NotNull(serviceIoC4);
		Ensure.NotNull(serviceIoC5);

		this.serviceIoC2 = serviceIoC2;
		this.serviceIoC3 = serviceIoC3;
		this.serviceIoC4 = serviceIoC4;
		this.serviceIoC5 = serviceIoC5;
		EnsureResolves();
	}

	private void EnsureResolves()
	{
		Ensure.NotNull(serviceIoC2);
		Ensure.NotNull(serviceIoC3);
		Ensure.NotNull(serviceIoC4);
		Ensure.NotNull(serviceIoC5);
	}
}
