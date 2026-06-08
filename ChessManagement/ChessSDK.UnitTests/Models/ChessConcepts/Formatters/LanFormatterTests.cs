namespace ChessSDK.UnitTests.Models.ChessConcepts.Formatters;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;

[TestClass]
public class LanFormatterTests
{
	[TestMethod]
	public void GivenAPawn_WhenCapturePromotion_ThenReturnsLan()
	{
		// Arrange
		var formatter = new LanFormatter();

		var move = new MoveModel(
			piece: PieceModel.Pawn,
			from: "e7",
			to: "d8",
			captured: PieceModel.Rook,
			promotion: PieceModel.Queen
		);

		// Act
		var lan = formatter.Format(move);

		// Assert
		lan.Should().Be("Pe7xd8Q");
	}

	[TestMethod]
	public void GivenAKnight_WhenSimpleMove_ThenReturnsLan()
	{
		// Arrange
		var formatter = new LanFormatter();

		var move = new MoveModel(
			piece: PieceModel.Knight,
			from: "g1",
			to: "f3"
		);

		// Act
		var lan = formatter.Format(move);

		// Assert
		lan.Should().Be("Ng1f3");
	}
}