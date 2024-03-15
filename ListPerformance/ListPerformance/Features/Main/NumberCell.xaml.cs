namespace ListPerformance.Features.Main;

public partial class NumberCell
{
	public NumberCell()
	{
		InitializeComponent();
	}


	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();

		if (BindingContext is NumberModel numberModel)
		{
			LblProperty1.Text = numberModel.Property1;
			LblProperty2.Text = numberModel.Property2;
			LblProperty3.Text = numberModel.Property3;
			LblProperty4.Text = numberModel.Property4;
			LblProperty5.Text = numberModel.Property5;
			LblProperty6.Text = numberModel.Property6;
			LblProperty7.Text = numberModel.Property7;
			LblProperty8.Text = numberModel.Property8;
			LblProperty9.Text = numberModel.Property9;
		}
	}
}