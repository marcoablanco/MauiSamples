namespace SessionizeInjectionWithScopes.Features.Dashboard;

using System;

public partial class DashboardPage
{
	public DashboardPage(IServiceProvider serviceProvider) : base(serviceProvider)
	{
		InitializeComponent();
	}
}