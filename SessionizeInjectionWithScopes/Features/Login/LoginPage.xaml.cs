namespace SessionizeInjectionWithScopes.Features.Login;

using System;

public partial class LoginPage
{
	private IServiceProvider serviceProvider;

	public LoginPage(IServiceProvider serviceProvider):base(serviceProvider)
	{
		this.serviceProvider = serviceProvider;
		InitializeComponent();
	}
}