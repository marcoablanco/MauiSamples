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
| `new_game` | Crea partida desde la posición inicial. Devuelve `gameId`, FEN, estado y tablero. |
| `get_position` | FEN, turno, estado (jaque, mate, tablas), número de legales y tablero ASCII. |
| `get_legal_moves` | Todos los movimientos legales agrupados por pieza, en SAN y notación larga. Admite `from` para filtrar por casilla. |
| `make_move` | Aplica un movimiento. Acepta SAN (`Nf3`, `exd5`, `O-O`, `e8=Q`) y notación larga (`e2e4`, `e7e8q`). |
| `undo_move` | Deshace los últimos `plies` movimientos (por defecto 1). |
| `get_history` | Historial en `san-en`, `san-es`, `figurine` o `lan`. |
| `list_games` | Partidas activas con su resultado. |
| `resign_game` | Abandona la partida. **La conserva** con su historial. |
| `delete_game` | Elimina la partida del servidor. Destructiva. |

`resign_game` y `delete_game` están separadas a propósito: un modelo no debe destruir el
historial creyendo que se rinde.

### Resources
- `chess://game/{gameId}/fen`
- `chess://game/{gameId}/board`

### Prompts
- `play_chess(style)` — configura al asistente para jugar. Obliga a consultar `get_legal_moves`
  antes de cada jugada.
- `analyze_position(gameId)` — pide análisis de la posición.

## Este proyecto no tiene lógica

Es un **adaptador**: sólo arranque, declaración de primitivas MCP y traducción de tipos.
Toda la lógica de ajedrez —y también la de presentación, como el texto del estado o la lista
de movimientos legales— vive en `ChessSDK`, porque una app MAUI o una web la necesitarían igual.

Por eso **no hay tests de este proyecto**: lo que hay que probar está en `ChessSDK.UnitTests`.
Si te ves escribiendo aquí un `if` sobre reglas o sobre cómo redactar algo, va en el SDK.

## Nota sobre el runtime

El paquete `ModelContextProtocol` requiere APIs de .NET 10 RTM.
Con el runtime `10.0.0-preview.3` instalado hace falta la referencia explícita a
`System.Text.Json 10.0.10` (ya incluida en el csproj). Al actualizar al runtime 10 final
esa referencia se puede eliminar.

