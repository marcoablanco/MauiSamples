namespace XamarinUITests.UITests
{
	using NUnit.Framework;
	using XamarinUITests.UITests.PageObjects;

	public class Horse : HorsePageObject
	{
		[SetUp]
		public void InitializeTests()
		{
			InitializeApp();
			NavigateToHorsePage();
		}

		[Test]
		public void GiveCorrectAnswer()
		{
			CheckCorrectScreen();
			PressAnswerButton();
			string answer = GetAnswerTextFromLabel();
			Assert.AreEqual("Blanco", answer);
		}
	}
}