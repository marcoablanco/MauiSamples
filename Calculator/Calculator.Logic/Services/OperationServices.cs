namespace Calculator.Logic.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class OperationServices : IOperationServices
{
	private readonly ILogger<OperationServices> logger;

	public OperationServices(IServiceProvider serviceProvider)
	{
		logger = serviceProvider.GetRequiredService<ILogger<OperationServices>>();
		logger.LogDebug("OperationServices created.");
	}

	public decimal Add(decimal firstOperand, decimal secondOperand)
	{
		logger.LogDebug("Adding {FirstOperand} and {SecondOperand}.", firstOperand, secondOperand);
		return firstOperand + secondOperand;
	}

	public decimal Subtract(decimal firstOperand, decimal secondOperand)
	{
		logger.LogDebug("Subtracting {SecondOperand} from {FirstOperand}.", secondOperand, firstOperand);
		return firstOperand - secondOperand;
	}

	public decimal Multiply(decimal firstOperand, decimal secondOperand)
	{
		logger.LogDebug("Multiplying {FirstOperand} and {SecondOperand}.", firstOperand, secondOperand);
		return firstOperand * secondOperand;
	}

	public decimal Divide(decimal firstOperand, decimal secondOperand)
	{
		logger.LogDebug("Dividing {FirstOperand} by {SecondOperand}.", firstOperand, secondOperand);
		if (secondOperand == 0)
			throw new DivideByZeroException();
		return firstOperand / secondOperand;
	}
}