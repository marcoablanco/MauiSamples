namespace SessionizeInjectionWithScopes;

using SessionizeInjectionWithScopes.Features.Login;

public partial class App : Application
{
	public App(LoginPage loginPage)
	{
		InitializeComponent();

		MainPage = loginPage;
	}
}
