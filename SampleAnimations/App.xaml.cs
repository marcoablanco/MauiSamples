namespace SampleAnimations;

using SampleAnimations.Features.Main;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainShell();
	}
}
