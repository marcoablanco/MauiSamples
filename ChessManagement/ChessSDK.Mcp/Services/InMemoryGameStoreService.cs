namespace ChessSDK.Mcp.Services;

using System.Collections.Concurrent;
using ChessSDK.Mcp.Models;
using ChessSDK.Models.ChessConcepts;

public sealed class InMemoryGameStoreService : IGameStoreService
{
	private readonly ConcurrentDictionary<string, GameSessionModel> games = new(StringComparer.OrdinalIgnoreCase);

	public GameSessionModel Create(GameColorModel humanColor)
	{
		var id = Guid.NewGuid().ToString("N")[..8];
		var session = new GameSessionModel(id, humanColor);
		games[id] = session;

		return session;
	}

	public GameSessionModel? Find(string gameId)
		=> string.IsNullOrWhiteSpace(gameId) ? null : games.GetValueOrDefault(gameId);

	public IReadOnlyCollection<GameSessionModel> All() => games.Values.ToArray();

	public bool Remove(string gameId) => games.TryRemove(gameId, out _);
}

