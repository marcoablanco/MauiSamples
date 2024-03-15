namespace ListPerformance.Features.Main;

public partial class MainPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		var items = new List<NumberModel>();

		for (int i = 0; i < 250; i++)
		{
			items.Add(new NumberModel
			{
				Property1 = i.ToString(),
				Property2 = i.ToString(),
				Property3 = i.ToString(),
				Property4 = i.ToString(),
				Property5 = i.ToString(),
				Property6 = i.ToString(),
				Property7 = i.ToString(),
				Property8 = i.ToString(),
				Property9 = i.ToString()
			});
		}


		ListWithBindings.ItemsSource = items;
		ListWithSetters.ItemsSource = items;
		CollectionWithBindings.ItemsSource = items;
		CollectionWithSetters.ItemsSource = items;
	}
}

