namespace Calculadora.App.Bases;

using Microsoft.Extensions.Logging;
using ReactiveUI;

public abstract class BaseViewModel : ReactiveObject
{
	private readonly ILogger<BaseViewModel> logger;

	protected BaseViewModel(IServiceProvider services)
	{
		logger = services.GetRequiredService<ILogger<BaseViewModel>>();
	}

	public virtual Task OnAppearingAsync()
	{
		logger.LogInformation("OnAppearingAsync called.");
		return Task.CompletedTask;
	}

	public virtual Task OnDisappearingAsync()
	{
		logger.LogInformation("OnDisappearingAsync called.");
		return Task.CompletedTask;
	}
}