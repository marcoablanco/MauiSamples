# AGENTS.md — Contexto del proyecto ChessManagement

> Este archivo lo leen automáticamente Copilot CLI y otros agentes. Contiene el estado real
> del repositorio y las decisiones ya tomadas. **Léelo entero antes de escribir código.**
> El trabajo pendiente está en [`PLAN.md`](./PLAN.md). Las convenciones de código, en
> [`.github/copilot-instructions.md`](./.github/copilot-instructions.md).

---

## 1. Qué estamos construyendo

Un **servidor MCP** que permite a un LLM local (vía Ollama) jugar al ajedrez.

**Tesis del proyecto:** un LLM alucina piezas y jugadas ilegales, pero si el servidor MCP
es la única fuente de verdad del estado y **valida cada movimiento antes de aplicarlo**,
el modelo no puede hacer trampas: como mucho recibe un error y reintenta.

Por eso la pieza crítica no es el prompt ni el modelo, sino el **generador de movimientos
legales** y la tool `get_legal_moves`. Sin eso, el proyecto no demuestra nada.

---

## 2. Estructura de la solución

```
ChessManagement.sln
├── ChessSDK/                  ← TODA la lógica de ajedrez
│   ├── Models/                   value objects, PositionModel, GameSessionModel
│   ├── Enums/                    MoveKindEnum, GameResultEnum
│   ├── Notation/                 FenSerializer, SanParser, PgnFormatter
│   ├── Rules/                    AttackDetector, MoveGenerator,
│   │                             LegalityValidator, GameResultEvaluator
│   └── Services/                 store de partidas
├── ChessSDK.Mcp/              ← adaptador MCP (stdio). Sin lógica.
├── ChessSDK.UnitTests/        ← MSTest + AwesomeAssertions (147 tests, en verde)
├── PLAN.md                    ← plan de implementación en 8 fases
└── .github/copilot-instructions.md
```

### Regla arquitectónica innegociable

`ChessSDK` es el **único** proyecto con lógica de ajedrez, porque mañana habrá una app MAUI
o una web consumiéndolo. Los proyectos de salida sólo pueden tener arranque, declaración de
primitivas y traducción de tipos.

**Prueba práctica:** si el código haría falta igualmente en una app MAUI, va en `ChessSDK`.

Esto **incluye la presentación de conceptos de dominio**: el texto del estado de la partida, la
lista de movimientos legales agrupada por pieza o el nombre español de una pieza son cosas que
una app o una web necesitarían igual. Van en `Formatters/`, no en el adaptador.

**Corolario sobre las pruebas:** los proyectos de salida (`ChessSDK.Mcp`, y mañana la app o la
web) son intermediarios y **no se prueban**. Si algo merece un test, es que es lógica, y entonces
es que va en `ChessSDK`. Verifica siempre contra `ChessSDK.UnitTests`; el sondeo por stdio sirve
para comprobar el protocolo una vez, no como bucle de desarrollo.

Ya hubo que corregir esto dos veces: `GameSessionModel`, `PlacedPieceModel` y el store estaban en
el proyecto MCP y se movieron al SDK; y en la Fase 5 se colaron en `ChessGameTools` la línea de
estado, el formateo de los movimientos legales y el bucle de deshacer N jugadas.
No repitas el error.

---

## 3. Estado actual del código

### `ChessSDK` — lo que existe

| Archivo | Qué es |
|---|---|
| `Models/Boards/FileModel.cs` | Columna a-h. Value object con igualdad, `Index` 0-7, instancias canónicas |
| `Models/Boards/RankModel.cs` | Fila 1-8. Igual, con `Index` 0-7 |
| `Models/Boards/CoordinateModel.cs` | Casilla. Igualdad por fichero+fila, `Index` 0-63, las 64 instancias cacheadas, `TryOffset` |
| `Models/Boards/PieceModel.cs` | Singleton sellado: Pawn…King, con `Symbol` y `Value` |
| `Models/Boards/PlacedPieceModel.cs` | Pieza + color, 12 instancias cacheadas (`Get`), `Symbol` FEN |
| `Models/Boards/CastlingRightsModel.cs` | Derechos de enroque inmutables, con parseo y texto FEN |
| `Models/Boards/PositionModel.cs` | **Estado real de la posición**: 64 casillas, turno, enroques, al paso, relojes. `Apply` inmutable |
| `Models/Boards/BoardModel.cs` | `AllFiles` / `AllRanks` |
| `Models/ChessConcepts/GameColorModel.cs` | White / Black, con `Opposite`, `Symbol`, `PawnDirection` |
| `Models/ChessConcepts/MoveModel.cs` | Pieza, origen, destino, captura, promoción y `Kind` (`MoveKindEnum`) |
| `Models/ChessConcepts/GameSessionModel.cs` | Envuelve `PositionModel` + historial. Valida contra los legales. `Undo()`, `TryResign()`, `Winner` |
| `Models/ChessConcepts/MatchModel.cs` | **Huérfano**, nadie lo usa |
| `Models/Players/PlayerModel.cs` | **Huérfano**, nadie lo usa |
| `Models/ChessConcepts/PgnHeadersModel.cs` | Las siete etiquetas obligatorias del PGN |
| `Models/ChessConcepts/Formatters/SanFormatterBase.cs` | Lógica SAN común: desambiguación, `O-O`, `+`, `#` |
| `Models/ChessConcepts/Formatters/*.cs` | SAN inglés, español, figurine, LAN (sólo el mapa de letras) |
| `Models/ChessConcepts/Formatters/MoveNotationFormatterFactory.cs` | Resuelve clave → formatter |
| `Models/ChessConcepts/Formatters/MoveHistoryFormatter.cs` | Numera: `1. e4 e5 2. Nf3` |
| `Models/ChessConcepts/Formatters/GameResultFormatter.cs` | Resultado en una frase: `jaque mate, ganan las negras` |
| `Models/ChessConcepts/Formatters/PieceNameFormatter.cs` | Nombre español de la pieza, singular y plural |
| `Enums/MoveKindEnum.cs` | Normal, DoublePawnPush, EnPassant, CastleKingSide, CastleQueenSide |
| `Enums/GameResultEnum.cs` | InProgress, Checkmate, Stalemate, InsufficientMaterial, ThreefoldRepetition, FiftyMoveRule, Resigned |
| `Notation/FenSerializer.cs` | `Serialize` / `Deserialize` / `TryDeserialize` (relojes opcionales) |
| `Notation/SanParser.cs` | Texto → `MoveModel`. Casa contra los legales escritos, no interpreta |
| `Notation/PgnFormatter.cs` | Exporta PGN: siete etiquetas, `[SetUp]`/`[FEN]`, movetext a 80 columnas |
| `Rules/AttackDetector.cs` | `IsSquareAttacked`. Primitiva de la que dependen jaque y enroque |
| `Rules/MoveGenerator.cs` | Pseudo-legales por pieza + enroque completamente validado |
| `Rules/LegalityValidator.cs` | Filtra los que dejan al rey propio en jaque. `IsInCheck` |
| `Rules/GameResultEvaluator.cs` | Mate, ahogado, material insuficiente, repetición triple, 50 jugadas |
| `Services/IGameStoreService.cs` + `InMemoryGameStoreService.cs` | Store de partidas en memoria |

### `ChessSDK.Mcp` — lo que existe

```
Program.cs              Host genérico + AddMcpServer + WithStdioServerTransport
                        + WithTools/Resources/PromptsFromAssembly
                        Logs a stderr (stdout es el canal MCP)
Tools/ChessGameTools.cs      9 tools: new_game, get_position, get_legal_moves, make_move,
                             undo_move, get_history, list_games, resign_game, delete_game
Resources/ChessResources.cs  chess://game/{gameId}/fen, chess://game/{gameId}/board
Prompts/ChessPrompts.cs      play_chess(style), analyze_position(gameId)
```

`resign_game` **marca** la partida como abandonada y la conserva; `delete_game` es la que borra.
Están separadas a propósito: un modelo no debe destruir el historial creyendo que se rinde.

Todas las tools de juego repiten la misma línea de estado, que es lo que el modelo debe mirar:
`Estado: en juego | Jaque: no | Movimientos legales: 20`.

Verificado funcionando: handshake correcto y `new_game` devuelve el FEN inicial exacto.

---

## 4. Lo que ya funciona y lo que falta

### Hecho (Fases 1 a 5 del `PLAN.md`)

- **Fase 1 — igualdad.** Todos los value objects tienen `Equals`/`GetHashCode`/`==`.
  `FileModel` y `RankModel` ya no convierten implícitamente a `string` (sólo a `char`),
  con lo que desaparece la ambigüedad de `StringBuilder.Append`.
- **Fase 2 — posición y FEN.** `PositionModel` es el estado real e inmutable;
  `FenSerializer` hace round-trip exacto sobre 20 FEN conocidos.
  `GameSessionModel` ya no tiene diccionario propio ni FEN artesanal.
- **Fase 3 — reglas. HITO SUPERADO.** `MoveGenerator` + `LegalityValidator` generan sólo
  movimientos legales, con enroque, al paso y promoción.
  **Perft correcto**: 20 / 400 / 8.902 / 197.281 / 4.865.609 desde la inicial y
  48 / 2.039 / 97.862 en Kiwipete, más otras cuatro posiciones de referencia.
- **Fase 4 — notación.** `SanFormatterBase` centraliza desambiguación, enroque y `+`/`#`;
  `SanParser` convierte texto en `MoveModel` **generando los legales y escribiéndolos**, no
  interpretando la cadena; `PgnFormatter` exporta la partida completa.
  `TryApplyMove` acepta ya `Nf3`, `exd5`, `O-O`, `e8=Q` además de `e2e4`.
- **Fase 5 — adaptador MCP.** 9 tools. `get_legal_moves` (con filtro por casilla) y `undo_move`
  (con `plies`) son nuevas; `get_position` incluye jaque, resultado y número de legales;
  `resign_game` marca en vez de borrar y `delete_game` es la que borra; `play_chess` obliga a
  consultar `get_legal_moves` antes de cada jugada.
- `GameSessionModel.TryApplyMove` rechaza cualquier jugada ilegal con un mensaje accionable
  (`'e2e5' no es legal. Movimientos legales de la pieza de 'e2': e2e3, e2e4.`) y expone
  `LegalMoves()`, `LegalMovesFrom()`, `IsInCheck`, `Result` y `Undo()`.

Verificado por stdio contra el servidor MCP real: `make_move e2e5` se rechaza, `e2e4` se aplica,
la secuencia SAN `e4 e5 Nf3 Nc6 Bb5` devuelve `1. e4 e5 2. Nf3 Nc6 3. Bb5` en `get_history`,
el mate del loco reporta `jaque mate, ganan las negras` y una partida abandonada rechaza
tanto mover como deshacer.

### Pendiente

- **Fase 6:** `draw_board` — `BoardAsciiFormatter` y `PieceLetterProvider` en el SDK
  (puede reaprovechar los mapas de `PieceNameFormatter` y de los formatters SAN),
  la tool, y actualizar el resource `chess://game/{id}/board`.
- **Fases 7 y 8:** tests de integración por stdio y cliente de Ollama.
- No hay `PgnParser`: se exporta PGN pero no se importa.

---

## 5. Entorno y comandos

```powershell
# Compilar
dotnet build ChessManagement.sln

# Tests (bucle rápido: sin los perft profundos)
dotnet test ChessSDK.UnitTests\ChessSDK.UnitTests.csproj --filter "TestCategory!=Slow"

# Tests completos, incluidos perft 4 y 5 (usa Release, es mucho más rápido)
dotnet test ChessSDK.UnitTests\ChessSDK.UnitTests.csproj -c Release

# Arrancar el servidor MCP
dotnet run --project ChessSDK.Mcp
```

### Probar el servidor a mano (stdio)

Redirigir con `cmd /c "... < in.txt > out.txt"` **no funciona** (el stdout se pierde al cerrar).
Hay que mantener stdin abierto:

```powershell
$exe = "C:\Git\GitHub\MauiSamples\ChessManagement\ChessSDK.Mcp\bin\Debug\net10.0\ChessSDK.Mcp.exe"
$psi = New-Object System.Diagnostics.ProcessStartInfo $exe
$psi.RedirectStandardInput=$true; $psi.RedirectStandardOutput=$true
$psi.RedirectStandardError=$true; $psi.UseShellExecute=$false
$p = [System.Diagnostics.Process]::Start($psi)
$p.StandardInput.WriteLine('{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"probe","version":"1.0"}}}')
$p.StandardInput.WriteLine('{"jsonrpc":"2.0","method":"notifications/initialized"}')
$p.StandardInput.WriteLine('{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"new_game","arguments":{"humanColor":"white"}}}')
$p.StandardInput.Flush()
1..2 | ForEach-Object { $p.StandardOutput.ReadLine() }
$p.Kill()
```

Alternativa: `npx @modelcontextprotocol/inspector`.

### Registro en el host
`.vscode/mcp.json` ya está configurado apuntando a `dotnet run --project ...`.

---

## 6. Entorno de la máquina

| | |
|---|---|
| SDK | .NET `10.0.100-preview.3` |
| Runtime | `Microsoft.NETCore.App 10.0.0-preview.3` (**no hay RTM instalado**) |
| GPU | RTX 3070 Laptop, **8 GB VRAM** |
| RAM | 32 GB |
| Ollama | 0.30.10, con `qwen2.5-coder:7b` |
| Shell | PowerShell 5.1 (usar `;` para encadenar, no `&&`) |

**Modelo recomendado y aún no descargado: `qwen3:8b`** (~5,2 GB, cabe entero en VRAM,
buen tool calling). `qwen2.5-coder` está afinado para código y no es la mejor opción.
Descartados por tamaño: `mistral-small3.2:24b`, `gpt-oss:20b`.

---

## 7. Trampas conocidas (te ahorrarán tiempo)

1. **`MissingMethodException: JsonElement.Parse`**
   El paquete MCP necesita APIs de .NET 10 RTM y el runtime instalado es preview 3.
   Solución aplicada: `<PackageReference Include="System.Text.Json" Version="10.0.10" />`
   en `ChessSDK.Mcp.csproj`. **Quitar ese pin cuando se instale el runtime .NET 10 final.**
   Cualquier proyecto nuevo que use MCP necesitará el mismo pin.

2. **Conversiones implícitas** (resuelto en Fase 1)
   `FileModel` y `RankModel` ya sólo convierten implícitamente a `char`; para texto se usa
   `ToString()` o la propiedad `Name`. No añadas de vuelta la conversión a `string`: con los
   operadores `==` definidos, provocaría CS0034 en las comparaciones.

3. **Nunca escribir en `Console.Out`** en el proyecto MCP: rompe el protocolo.
   Los logs ya van a stderr en `Program.cs`.

4. **Caracteres Unicode de recuadro** (Fase 6, `draw_board`): hará falta
   `Console.OutputEncoding = Encoding.UTF8` en `Program.cs`.

5. **Perft es la red de seguridad.** Si tocas `MoveGenerator`, `AttackDetector`,
   `LegalityValidator` o `PositionModel.Apply`, ejecuta `PerftTests` completo
   (incluida la categoría `Slow`) antes de dar nada por bueno.

6. Los tests de profundidad 4 y 5 están marcados `[TestCategory("Slow")]`. Para el bucle
   rápido: `dotnet test ... --filter "TestCategory!=Slow"`.

7. Un cliente MCP puede recibir las respuestas **fuera de orden** si envía varias llamadas
   sin esperar: el servidor las atiende en paralelo. Al probar a mano, lee cada respuesta
   antes de enviar la siguiente.

8. **Los dialectos SAN chocan.** En español `R` es Rey; en inglés `R` es Rook (torre).
   Por eso `SanParser` recibe **un solo** formatter (inglés por defecto) en vez de aceptar
   todos a la vez: aceptarlos juntos convertiría cada `R` en ambiguo.

9. `IMoveNotationFormatter` tiene dos sobrecargas: `Format(move)` escribe la forma corta sin
   contexto y `Format(move, position)` añade desambiguación y `+`/`#`. Usa siempre la segunda
   si tienes la posición; la primera existe para no romper lo que ya la usaba.

---

## 8. Convenciones de código (resumen)

Detalle completo en `.github/copilot-instructions.md`. Lo esencial:

- **Orden de miembros**: campos → constructores → propiedades → métodos static →
  abstract → métodos de instancia → overrides. Dentro de cada grupo: public → internal →
  protected → private.
- **Sufijos obligatorios** en archivo y clase, y el archivo se llama como el tipo:
  `Model`, `Service`, `Repository`, `Formatter`, `Parser`, `Serializer`, `Generator`,
  `Validator`, `Factory`, `Extensions`, `ViewModel`, `Tools`/`Resources`/`Prompts`,
  `Tests`, `IntegrationTests`. Interfaces: `I` + sufijo → `IGameStoreService`.
- Prohibidos `Helper`, `Manager`, `Util`, `Common`.
- La carpeta concuerda con el sufijo. Única excepción: `Program.cs`.
- **Estilo observado en el repo**: tabuladores, namespaces file-scoped, `using` **dentro**
  del namespace, un tipo público por archivo.
- **Tests**: MSTest + AwesomeAssertions, nombres `GivenX_WhenY_ThenZ`, estructura
  Arrange / Act / Assert con comentarios.

---

## 9. Estado de git y regla de oro

### La IA no toca el repositorio

El repositorio es la herramienta del **usuario** para revisar los cambios del agente.
Cualquier operación de escritura en git ensucia esa revisión y está prohibida.

**Nunca ejecutes:** `git commit`, `git push`, `git add` / staging, `git reset`, `git revert`,
`git checkout`, `git restore`, `git stash`, `git merge`, `git rebase`, `git cherry-pick`,
`git branch`, `git switch`, `git tag`, `git clean`.

**Sí puedes** usar comandos de sólo lectura: `git status`, `git diff`, `git log`, `git show`.

Los cambios se dejan en el árbol de trabajo, **sin indexar**. El usuario decide qué se
commitea y cuándo. Si crees que conviene un commit, propónlo; no lo hagas.

### Estado

- Rama `main`, con cambios pendientes de commitear por el usuario: la Fase 5 (abandono en el SDK,
  `get_legal_moves`, `undo_move`, `delete_game`, línea de estado y prompt endurecido) y la
  documentación de las Fases 4 y 5.

---

## 10. Por dónde continuar

Siguiente paso: **Fase 6 del `PLAN.md`** — `draw_board`: `BoardAsciiFormatter` y
`PieceLetterProvider` en el SDK, la tool que delega en ellos y el resource
`chess://game/{id}/board`. Acuérdate de `Console.OutputEncoding = Encoding.UTF8`.

Ruta crítica (Fases 1 a 5 **completadas**):

```
Fase 1 igualdad ✔ → Fase 2 PositionModel+FEN ✔ → Fase 3 MoveGenerator+perft ✔ ← HITO
     → Fase 4 SAN/PGN ✔ → Fase 5 migrar MCP ✔
     → Fase 6 draw_board   ← siguiente
     → Fase 7 tests integración → Fase 8 Ollama
```

El hito que valida el proyecto era **perft correcto en Fase 3**: 20 / 400 / 8.902 / 197.281
nodos a profundidad 1-4 desde la posición inicial. Está verificado, junto con Kiwipete y
otras cuatro posiciones trampa. Si alguna vez deja de cuadrar, el generador está mal
y el MCP dará movimientos ilegales por buenos.

