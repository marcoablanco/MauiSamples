namespace XamarinUITests.UITests
{
	using NUnit.Framework;
	using System;
	using System.IO;
	using Xamarin.UITest;

	public class AppInitializer
	{
		public static IApp StartApp(Platform platform)
		{
			if (platform == Platform.Android)
			{
				return ConfigureApp.Android
								   .EnableLocalScreenshots()
								   .PreferIdeSettings()
								   .InstalledApp("com.companyname.xamarinuitests")
								   .StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
			}
			return null;
		}
	}
}
