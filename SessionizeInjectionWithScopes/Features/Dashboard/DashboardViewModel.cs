namespace SessionizeInjectionWithScopes.Features.Dashboard;

using SessionizeInjectionWithScopes.Bases;
using System;

public class DashboardViewModel : BasePageViewModel
{
	public DashboardViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
	{
	}
}
