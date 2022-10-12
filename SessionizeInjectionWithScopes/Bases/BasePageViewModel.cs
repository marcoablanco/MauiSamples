namespace SessionizeInjectionWithScopes.Bases;

using RxUI.MauiToolkit.Bases;

public class BasePageViewModel : RxBasePageViewModel
{
	public BasePageViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
	{

	}
}
