namespace SampleLog;

using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

internal class CustomLogger : ILogger
{
	public CustomLogger()
	{
	}

	public string CategoryName { get; internal set; }

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		string line = $"{logLevel} - {eventId.Id} - {CategoryName} - {formatter(state, exception)}";
		Debug.WriteLine(line);
	}
}
