namespace Calculator.Logic.Tests.Services;

using Calculator.Logic.Services;
using FluentAssertions;

[TestClass]
public class OperationServiceTests : BaseService<OperationService>
{
	[TestInitialize]
	public void Setup()
	{
		Init();
		service = new OperationService(serviceProvider);
	}

	[TestMethod]
	public void GivenTwoNumbers_WhenCallSum_ThenReturnSum()
	{
		// Arrange
		decimal a = 5;
		decimal b = 3;
		decimal expected = 8;

		// Act
		var result = service.Add(a, b);

		// Assert
		result.Should().Be(expected);
	}
}