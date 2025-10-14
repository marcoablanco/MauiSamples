namespace Calculator.Maui.Features.Calculator;

using global::Calculator.Logic.Services;
using global::Calculator.Maui.Bases;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Globalization;

public partial class CalculatorViewModel : BaseViewModel
{
	private readonly ILogger<CalculatorViewModel> logger;
	private readonly IOperationService operationService;

	private string currentInput;
	private string operatorValue;
	private double? firstOperand;

	public CalculatorViewModel(IServiceProvider services) : base(services)
	{
		logger = services.GetRequiredService<ILogger<CalculatorViewModel>>();
		operationService = services.GetRequiredService<IOperationService>();

		currentInput = string.Empty;
		operatorValue = string.Empty;
		firstOperand = null;
		Display = "0";

		PressButtonCommand = ReactiveCommand.Create<string, string>(PressButtonCommandExecute);
	}

	[Reactive]
	public partial string Display { get; set; }

	public ReactiveCommand<string, string> PressButtonCommand { get; }


	private string PressButtonCommandExecute(string arg)
	{
		logger.LogDebug("Button pressed: {Button}", arg);

		if (string.IsNullOrEmpty(arg))
			return Display;

		if (char.IsDigit(arg[0]))
			return HandleDigit(arg);

		switch (arg)
		{
			case ".":
				return HandleDot();
			case "+" or "-" or "*" or "/":
				return HandleOperator(arg);
			case "=":
				return HandleEquals();
			case "C":
				return HandleClear();
			case "D":
				return HandleDelete();
			default:
				return Display;
		}
	}

	private string HandleDigit(string digit)
	{
		if (currentInput == "0")
			currentInput = string.Empty;
		currentInput += digit;
		Display = currentInput;
		return Display;
	}

	private string HandleDot()
	{
		if (currentInput.Contains('.'))
			return Display;

		if (string.IsNullOrEmpty(currentInput))
			currentInput = "0";

		currentInput += ".";
		Display = currentInput;

		return Display;
	}

	private string HandleOperator(string op)
	{
		if (firstOperand != null || !double.TryParse(currentInput, out var val))
			return Display;

		firstOperand = val;
		operatorValue = op;
		currentInput = "";

		return Display;
	}

	private string HandleEquals()
	{
		if (firstOperand == null || string.IsNullOrEmpty(operatorValue) || !decimal.TryParse(currentInput, out var second))
			return Display;

		try
		{
			decimal result = 0;
			var first = Convert.ToDecimal(firstOperand.Value);
			result = operatorValue switch
					 {
						 "+" => operationService.Add(first, second),
						 "-" => operationService.Subtract(first, second),
						 "*" => operationService.Multiply(first, second),
						 "/" => operationService.Divide(first, second),
						 _ => result
					 };
			Display = result.ToString(CultureInfo.InvariantCulture);
		}
		catch (DivideByZeroException)
		{
			Display = "Error: Division by zero";
		}
		firstOperand = null;
		operatorValue = "";
		currentInput = "";
		return Display;
	}

	private string HandleClear()
	{
		Display = "0";
		currentInput = "";
		firstOperand = null;
		operatorValue = "";
		return Display;
	}

	private string HandleDelete()
	{
		if (!string.IsNullOrEmpty(currentInput))
		{
			currentInput = currentInput[..^1];
			Display = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
		}

		return Display;
	}
}