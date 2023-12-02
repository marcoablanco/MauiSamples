namespace SampleLog;

using Microsoft.Extensions.Logging;

public partial class MainPage : ContentPage
{
	private readonly ILogger<MainPage> logger;
	int count = 0;

	public MainPage(ILogger<MainPage> logger)
	{
		this.logger = logger;

		InitializeComponent();

		logger.LogInformation("MainPage created.");
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;
		logger.LogDebug($"Counter clicked: {count}.");

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

