namespace SampleAnimations.Features.MicrosoftMauiControls;

using Microsoft.Maui.Controls;
using SampleAnimations.Resources.Translations;

public partial class SampleAnimationPage : ContentPage
{
	private Animation animation;
	private bool isExecutingAnimation;

	public SampleAnimationPage()
	{
		InitializeComponent();
		BtnStart.Command = new Command(() => BtnStartCommandExecute());
	}

	private void BtnStartCommandExecute()
	{
		if (isExecutingAnimation)
		{
			if (animation is not null)
			{
				animation.Pause();
				animation.Repeats = false;
				animation.Reset();
				animation.Dispose();
			}
			BtnStart.Text = Text.StartAnimation;
			isExecutingAnimation = false;
		}
		else
		{
			animation = new Animation
			{
				{ 0, 0.5, new Animation(x => LblShowAnimation.TranslationY = x, 0, 50, Easing.CubicInOut) },
				{ 0.5, 1, new Animation(x => LblShowAnimation.TranslationY = x, 50, 0, Easing.CubicInOut) },
			};
			animation.Commit(this, "MyAnimation", length: 2000, repeat: () => true);
			BtnStart.Text = Text.StopAnimation;
			isExecutingAnimation = true;
		}
	}
}