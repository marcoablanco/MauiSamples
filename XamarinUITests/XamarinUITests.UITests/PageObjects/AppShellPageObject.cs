namespace XamarinUITests.UITests.PageObjects
{
	using Xamarin.UITest;

	public class AppShellPageObject : BaseTests
	{
		private string menuButton = "Open navigation drawer";
		private string menuOptionHorse = "Horse";

		public AppShellPageObject() : base()
		{

		}

		public AppShellPageObject(IApp app) : base(app)
		{
		}

		public void OpenMenu()
		{
			Tap(menuButton);
		}

		public void PressInHorseMenu()
		{
			Tap(menuOptionHorse);
		}
	}
}
