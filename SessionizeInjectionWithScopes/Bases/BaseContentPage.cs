namespace SessionizeInjectionWithScopes.Bases;

using RxUI.MauiToolkit.Bases;

public class BaseContentPage<TViewModel> : RxBaseContentPage<TViewModel> where TViewModel : BasePageViewModel
{
	public BaseContentPage(IServiceProvider serviceProvider):base(serviceProvider)
	{
	}
}
