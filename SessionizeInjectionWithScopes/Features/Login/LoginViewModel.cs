namespace SessionizeInjectionWithScopes.Features.Login;

using ReactiveUI;
using RxUI.MauiToolkit.Services.AppLog;
using SessionizeInjectionWithScopes.Bases;
using SessionizeInjectionWithScopes.Common;
using SessionizeInjectionWithScopes.Services.Authentication;
using SessionizeInjectionWithScopes.Services.NavigationPage;
using System;
using System.Reactive;
using System.Threading.Tasks;

public class LoginViewModel : BasePageViewModel
{
	private readonly IAuthenticationService authenticationService;
	private readonly ILogService<LoginViewModel> logService;
	private readonly IServiceProvider serviceProvider;
	private string username;
	private string password;

	public LoginViewModel(IServiceProvider serviceProvider):base(serviceProvider)
	{
		authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
		logService = serviceProvider.GetRequiredService<ILogService<LoginViewModel>>();

		username = string.Empty;
		password = string.Empty;
		LoginCommand = ReactiveCommand.CreateFromTask(LoginCommandExecuteAsync);
		this.serviceProvider = serviceProvider;
	}

	public string Username
	{
		get => username;
		set => this.RaiseAndSetIfChanged(ref username, value);
	}

	public string Password
	{
		get => password;
		set => this.RaiseAndSetIfChanged(ref password, value);
	}

	public ReactiveCommand<Unit, Unit> LoginCommand { get; }

	private async Task LoginCommandExecuteAsync()
	{
		try
		{
			await authenticationService.GetTokenAsync(Username, Password);
			AppSession.CreateSession(Username, serviceProvider);
			INavigationPageService mainShell = AppSession.Current.Resolve<INavigationPageService>();
		}
		catch (Exception ex)
		{
			logService.Error(ex);
		}
	}
}
