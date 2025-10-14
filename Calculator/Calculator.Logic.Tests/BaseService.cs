namespace Calculator.Logic.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

public abstract class BaseService<TService>
{
#pragma warning disable CS8618
	protected TService service;

	protected IServiceProvider serviceProvider;
	protected ILogger genericLogger;
	protected ILogger<TService> logger;
#pragma warning restore CS8618

	public virtual void Init()
	{
		serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();

		logger = Substitute<ILogger<TService>>();
		genericLogger = Substitute<ILogger>();
	}

	public T Substitute<T>() where T : class
	{
		serviceProvider.Should().NotBeNull();

		var service = NSubstitute.Substitute.For<T>();

		serviceProvider.GetService(typeof(T)).Returns(service);
		return service;
	}
}