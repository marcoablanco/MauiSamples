namespace SessionizeInjectionWithScopes.Features.Main;

using SessionizeInjectionWithScopes.Services.NavigationPage;

public partial class MainShell : Shell, INavigationPageService
{
	public MainShell()
	{
		InitializeComponent();
	}
}
