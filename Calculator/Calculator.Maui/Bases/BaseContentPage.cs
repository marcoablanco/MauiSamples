using Microsoft.Extensions.Logging;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Calculator.Maui.Bases;

using ReactiveUI.Maui;

public class BaseContentPage<TViewModel> : ReactiveContentPage<TViewModel>
	where TViewModel : BaseViewModel
{
	private readonly ILogger<BaseContentPage<TViewModel>> logger;

	protected BaseContentPage(IServiceProvider services)
	{
		logger = services.GetRequiredService<ILogger<BaseContentPage<TViewModel>>>();
		ViewModel = services.GetRequiredService<TViewModel>();

		this.WhenActivated(OnActivated);
	}

	public new TViewModel ViewModel
	{
		get => base.ViewModel!;
		set => base.ViewModel = value;
	}

	protected override async void OnAppearing()
	{
		try
		{
			base.OnAppearing();
			await ViewModel.OnAppearingAsync();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error in OnAppearing.");
		}
	}

	protected override async void OnDisappearing()
	{
		try
		{
			base.OnDisappearing();
			await ViewModel.OnDisappearingAsync();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error in OnDisappearing.");
		}
	}

	protected virtual void OnActivated(CompositeDisposable disposables)
	{
	}
}