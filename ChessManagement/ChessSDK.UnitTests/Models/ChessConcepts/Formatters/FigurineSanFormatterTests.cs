namespace ChessSDK.UnitTests.Models.ChessConcepts.Formatters;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;

[TestClass]
public class FigurineSanFormatterTests
{
	[TestMethod]
	public void GivenAPawn_WhenCapturePromotion_ThenReturnsFigurineSan()
	{
		// Arrange
		var formatter = new FigurineSanFormatter();

		var move = new MoveModel(
			piece: PieceModel.Pawn,
			from: "e7",
			to: "d8",
			captured: PieceModel.Rook,
			promotion: PieceModel.Queen
		);

		// Act
		var san = formatter.Format(move);

		// Assert
		san.Should().Be("exd8=♕");
	}

	[TestMethod]
	public void GivenAKnight_WhenSimpleMove_ThenReturnsFigurineSan()
	{
		// Arrange
		var formatter = new FigurineSanFormatter();

		var move = new MoveModel(
			piece: PieceModel.Knight,
			from: "g1",
			to: "f3"
		);

		// Act
		var san = formatter.Format(move);

		// Assert
		san.Should().Be("♘f3");
	}
}