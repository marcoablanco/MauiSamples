namespace ChessSDK.Mcp.Services;

using ChessSDK.Mcp.Models;
using ChessSDK.Models.ChessConcepts;

public interface IGameStoreService
{
	GameSessionModel Create(GameColorModel humanColor);
	GameSessionModel? Find(string gameId);
	IReadOnlyCollection<GameSessionModel> All();
	bool Remove(string gameId);
}
