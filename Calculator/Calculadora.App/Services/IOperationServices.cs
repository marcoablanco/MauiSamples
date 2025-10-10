namespace Calculadora.App.Services;

public interface IOperationServices
{
	decimal Add(decimal firstOperand, decimal secondOperand);
	decimal Subtract(decimal firstOperand, decimal secondOperand);
	decimal Multiply(decimal firstOperand, decimal secondOperand);
	decimal Divide(decimal firstOperand, decimal secondOperand);
}