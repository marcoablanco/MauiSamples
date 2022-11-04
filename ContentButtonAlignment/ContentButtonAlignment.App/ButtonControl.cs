namespace ContentButtonAlignment.App;

using Microsoft.Maui.Handlers;
using System.Runtime.CompilerServices;

public class ButtonControl : Button
{
	public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(ButtonControl));
	public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(nameof(VerticalTextAlignment), typeof(TextAlignment), typeof(ButtonControl));

	public ButtonControl()
	{
	}

	public TextAlignment HorizontalTextAlignment
	{
		get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
		set => SetValue(HorizontalTextAlignmentProperty, value);
	}

	public TextAlignment VerticalTextAlignment
	{
		get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
		set => SetValue(VerticalTextAlignmentProperty, value);
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		OnTextAlignmentPropertyChanged();
	}

	protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == HorizontalTextAlignmentProperty.PropertyName || propertyName == VerticalTextAlignmentProperty.PropertyName)
			OnTextAlignmentPropertyChanged();
	}

private void OnTextAlignmentPropertyChanged()
{
	if (Handler is ButtonHandler buttonHandler)
	{
		OnTextAlignmentPropertyChangedAndroid(buttonHandler);
		OnTextAlignmentPropertyChangedWindows(buttonHandler);
		OnTextAlignmentPropertyChangedIOS(buttonHandler);
	}
}

	private void OnTextAlignmentPropertyChangedAndroid(ButtonHandler buttonHandler)
	{
#if ANDROID
		Android.Views.GravityFlags horizontalFlag = HorizontalTextAlignment switch
		{
			TextAlignment.Start => Android.Views.GravityFlags.Left,
			TextAlignment.Center => Android.Views.GravityFlags.CenterHorizontal,
			TextAlignment.End => Android.Views.GravityFlags.Right,
			_ => Android.Views.GravityFlags.Center
		};
		Android.Views.GravityFlags verticalFlag = VerticalTextAlignment switch
		{
			TextAlignment.Start => Android.Views.GravityFlags.Top,
			TextAlignment.Center => Android.Views.GravityFlags.CenterVertical,
			TextAlignment.End => Android.Views.GravityFlags.Bottom,
			_ => Android.Views.GravityFlags.Center
		};
		buttonHandler.PlatformView.Gravity = horizontalFlag | verticalFlag;
#endif
	}

	private void OnTextAlignmentPropertyChangedWindows(ButtonHandler buttonHandler)
	{
#if WINDOWS
		buttonHandler.PlatformView.HorizontalContentAlignment = HorizontalTextAlignment switch
		{
			TextAlignment.Start => Microsoft.UI.Xaml.HorizontalAlignment.Left,
			TextAlignment.Center => Microsoft.UI.Xaml.HorizontalAlignment.Center,
			TextAlignment.End => Microsoft.UI.Xaml.HorizontalAlignment.Right,
			_ => Microsoft.UI.Xaml.HorizontalAlignment.Center,
		};
		buttonHandler.PlatformView.VerticalContentAlignment = VerticalTextAlignment switch
		{
			TextAlignment.Start => Microsoft.UI.Xaml.VerticalAlignment.Top,
			TextAlignment.Center => Microsoft.UI.Xaml.VerticalAlignment.Center,
			TextAlignment.End => Microsoft.UI.Xaml.VerticalAlignment.Bottom,
			_ => Microsoft.UI.Xaml.VerticalAlignment.Center,
		};
#endif
	}

private void OnTextAlignmentPropertyChangedIOS(ButtonHandler buttonHandler)
{
#if IOS
	buttonHandler.PlatformView.HorizontalAlignment = HorizontalTextAlignment switch
	{
		TextAlignment.Start => UIKit.UIControlContentHorizontalAlignment.Left,
		TextAlignment.Center => UIKit.UIControlContentHorizontalAlignment.Center,
		TextAlignment.End => UIKit.UIControlContentHorizontalAlignment.Right,
		_ => UIKit.UIControlContentHorizontalAlignment.Center,
	};
	buttonHandler.PlatformView.VerticalAlignment = HorizontalTextAlignment switch
	{
		TextAlignment.Start => UIKit.UIControlContentVerticalAlignment.Top,
		TextAlignment.Center => UIKit.UIControlContentVerticalAlignment.Center,
		TextAlignment.End => UIKit.UIControlContentVerticalAlignment.Bottom,
		_ => UIKit.UIControlContentVerticalAlignment.Center,
	};
#endif
}
}