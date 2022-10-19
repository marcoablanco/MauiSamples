namespace XamarinUITests.UITests.PageObjects
{
	using System;
	using XamarinUITests.UITests;

	public class HorsePageObject : BaseTests
	{
		private string namePage = "Wiki Caballos";
		private string btnAnswerText = "Respuesta";
		private string lblAnswer = "LblAnswer";

		public void NavigateToHorsePage()
		{
			AppShellPageObject appShellPage = new AppShellPageObject(app);
			appShellPage.OpenMenu();
			appShellPage.PressInHorseMenu();
		}

		public void CheckCorrectScreen()
		{
			CheckByText(namePage);
		}

		protected void PressAnswerButton()
		{
			TapByText(btnAnswerText);
		}

		protected string GetAnswerTextFromLabel()
		{
			return GetText(lblAnswer);
		}
	}
}
