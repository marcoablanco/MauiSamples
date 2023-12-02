namespace SampleLog;

using Microsoft.Extensions.Logging;


[ProviderAlias("CustomLogger")]
internal class CustomLoggerProvider : ILoggerProvider
{
	private readonly IServiceProvider serviceProvider;

	public CustomLoggerProvider(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public ILogger CreateLogger(string categoryName)
	{
		CustomLogger logger = serviceProvider.GetRequiredService<CustomLogger>();
		logger.CategoryName = categoryName;
		return logger;
	}

	public void Dispose()
	{
	}
}
