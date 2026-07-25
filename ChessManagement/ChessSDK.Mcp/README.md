# ChessSDK.Mcp

Servidor MCP (stdio) que expone la partida de ajedrez al modelo.

## Ejecutar

```powershell
dotnet run --project ChessSDK.Mcp
```

El proceso habla JSON-RPC por **stdout**; todos los logs van a **stderr** (configurado en `Program.cs`).

## Primitivas expuestas

### Tools
| Nombre | Descripción |
|---|---|
| `new_game` | Crea partida desde la posición inicial. Devuelve `gameId`, FEN y tablero. |
| `get_position` | FEN + turno + tablero ASCII. |
| `make_move` | Aplica un movimiento en notación larga (`e2e4`, `e7e8q`). |
| `get_history` | Historial en `san-en`, `san-es`, `figurine` o `lan`. |
| `list_games` | Partidas activas. |
| `resign_game` | Abandona y elimina la partida. |

### Resources
- `chess://game/{gameId}/fen`
- `chess://game/{gameId}/board`

### Prompts
- `play_chess(style)` — configura al asistente para jugar.
- `analyze_position(gameId)` — pide análisis de la posición.

## Limitaciones actuales

`GameSessionModel` sólo valida: casilla de origen ocupada, turno correcto y no capturar pieza propia.**No** hay generación de movimientos legales, jaque, mate, enroque, al paso ni repetición.
Eso pertenece a `ChessSDK` y está pendiente:

1. `PositionModel` + `FenSerializer`.
2. `MoveGenerator` + validación de jaque (con tests de `perft`).
3. Parser SAN con desambiguación.
4. Tool `get_legal_moves` y detección de fin de partida.

## Nota sobre el runtime

El paquete `ModelContextProtocol` requiere APIs de .NET 10 RTM.
Con el runtime `10.0.0-preview.3` instalado hace falta la referencia explícita a
`System.Text.Json 10.0.10` (ya incluida en el csproj). Al actualizar al runtime 10 final
esa referencia se puede eliminar.

