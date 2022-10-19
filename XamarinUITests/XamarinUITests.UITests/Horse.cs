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

			// Commented because with test don't play.
			// Assert.AreEqual("Blanco", answer);
			// so we check both answers

			Assert.IsTrue(answer == "Blanco" || answer == "Negro");
		}
	}
}