namespace SampleAnimations.Features.MicrosoftMauiAnimations;

using Microsoft.Maui.Animations;
using SampleAnimations.Resources.Translations;

public partial class SampleAnimationPage
{
	private bool isExecutingAnimation;
	private Animation animation;

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
			animation.Repeats = true;
			Ticker ticker = new Ticker
			{
				MaxFps = 30
			};
			AnimationManager animationManger = new AnimationManager(ticker)
			{
				AutoStartTicker = true,
				SpeedModifier = 1,
			};
			animation.Commit(animationManger);
			BtnStart.Text = Text.StopAnimation;
			isExecutingAnimation = true;
		}
	}
}