namespace XamarinUITests;

using System;

public partial class HorsePage : ContentPage
{
	public HorsePage()
	{
		InitializeComponent();

		BtnAnswer.Command = new Command(() => BtnAnswerCommandExecuteAsync());
	}

	private void BtnAnswerCommandExecuteAsync()
	{
		Random random = new Random();

		if (random.Next(1, 3) == 1)
			LblAnswer.Text = "Negro";
		else
			LblAnswer.Text = "Blanco";
	}
}