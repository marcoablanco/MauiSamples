namespace Calculadora.App.Features.Calculator;

using ReactiveUI;
using System.Reactive.Disposables;

public partial class CalculatorPage
{
	public CalculatorPage(IServiceProvider serviceProvider) : base(serviceProvider)
	{
		InitializeComponent();
	}

	protected override void OnActivated(CompositeDisposable disposables)
	{
		base.OnActivated(disposables);

		disposables.Add(this.OneWayBind(ViewModel, vm => vm.Display, v => v.LblResult.Text));

		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn0));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn1));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn2));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn3));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn4));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn5));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn6));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn7));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn8));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.Btn9));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnSum));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnSubtract));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnMultiply));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnDivide));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnDot));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnEqual));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnClean));
		disposables.Add(this.BindCommand(ViewModel, vm => vm.PressButtonCommand, v => v.BtnDelete));
	}
}