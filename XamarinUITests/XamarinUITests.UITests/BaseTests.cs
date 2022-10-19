namespace XamarinUITests.UITests
{
	using System;
	using System.Linq;
	using Xamarin.UITest;
	using Xamarin.UITest.Queries;

	public class BaseTests
	{
		protected IApp app;

		public BaseTests()
		{
		}

		public BaseTests(IApp app)
		{
			this.app = app;
		}

		protected void InitializeApp()
		{
			app = AppInitializer.StartApp(Platform.Android);
		}

		protected void Tap(string nameElement)
		{
			app.Tap(nameElement);
		}

		/// <summary>
		/// Be careful, my young padawan, can be many object with the same text.
		/// </summary>

		protected void TapByText(string text)
		{
			app.Tap(q=> q.Text(text));
		}

		protected bool Check(string nameElement)
		{
			AppResult[] result = app.WaitForElement(nameElement, $"Not found: {nameElement}", TimeSpan.FromSeconds(3));
			return result != null;
		}

		/// <summary>
		/// Be careful, my young padawan, can be many object with the same text.
		/// </summary>
		protected bool CheckByText(string text)
		{
			AppResult[] result = app.Query(q => q.Text(text));
			return result != null && result.Any();
		}

		protected string GetText(string nameElement)
		{
			AppResult[] result = app.WaitForElement(nameElement, $"Not found: {nameElement}", TimeSpan.FromSeconds(3));
			if (result != null && result.Any())
				return result[0].Text;
			else
				throw new NullReferenceException("Element not found.");
		}

	}
}
