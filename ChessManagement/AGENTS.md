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
├── ChessSDK.Mcp/              ← adaptador MCP (stdio). Sin lógica.
├── ChessSDK.UnitTests/        ← MSTest + AwesomeAssertions (12 tests, en verde)
├── PLAN.md                    ← plan de implementación en 8 fases
└── .github/copilot-instructions.md
```

### Regla arquitectónica innegociable

`ChessSDK` es el **único** proyecto con lógica de ajedrez, porque mañana habrá una app MAUI
o una web consumiéndolo. Los proyectos de salida sólo pueden tener arranque, declaración de
primitivas y traducción de tipos.

**Prueba práctica:** si el código haría falta igualmente en una app MAUI, va en `ChessSDK`.

Ya hubo que corregir esto una vez: `GameSessionModel`, `PlacedPieceModel` y el store estaban
en el proyecto MCP y se movieron al SDK. No repitas el error.

---

## 3. Estado actual del código

### `ChessSDK` — lo que existe

| Archivo | Qué es |
|---|---|
| `Models/Boards/FileModel.cs` | Columna a-h. Instancias estáticas + conversiones implícitas |
| `Models/Boards/RankModel.cs` | Fila 1-8. Igual + propiedad `Index` |
| `Models/Boards/CoordinateModel.cs` | Casilla. Factory `Create` + conversión desde `"e4"` |
| `Models/Boards/PieceModel.cs` | Enum-like: Pawn, Knight, Bishop, Rook, Queen, King |
| `Models/Boards/PlacedPieceModel.cs` | Pieza + color (`PieceModel` no tiene color) |
| `Models/Boards/BoardModel.cs` | `AllFiles` / `AllRanks` |
| `Models/ChessConcepts/GameColorModel.cs` | White / Black |
| `Models/ChessConcepts/MoveModel.cs` | Pieza, origen, destino, captura, promoción |
| `Models/ChessConcepts/GameSessionModel.cs` | Estado de partida **provisional** (ver abajo) |
| `Models/ChessConcepts/MatchModel.cs` | **Huérfano**, nadie lo usa |
| `Models/Players/PlayerModel.cs` | **Huérfano**, nadie lo usa |
| `Models/ChessConcepts/Formatters/*.cs` | SAN inglés, español, figurine, LAN |
| `Models/ChessConcepts/Formatters/MoveNotationFormatterFactory.cs` | Resuelve clave → formatter |
| `Models/ChessConcepts/Formatters/MoveHistoryFormatter.cs` | Numera: `1. e4 e5 2. Nf3` |
| `Services/IGameStoreService.cs` + `InMemoryGameStoreService.cs` | Store de partidas en memoria |

### `ChessSDK.Mcp` — lo que existe

```
Program.cs              Host genérico + AddMcpServer + WithStdioServerTransport
                        + WithTools/Resources/PromptsFromAssembly
                        Logs a stderr (stdout es el canal MCP)
Tools/ChessGameTools.cs      new_game, get_position, make_move,
                             get_history, list_games, resign_game
Resources/ChessResources.cs  chess://game/{gameId}/fen, chess://game/{gameId}/board
Prompts/ChessPrompts.cs      play_chess(style), analyze_position(gameId)
```

Verificado funcionando: handshake correcto y `new_game` devuelve el FEN inicial exacto.

---

## 4. ⚠️ Lo que NO funciona todavía (crítico)

`GameSessionModel` es una **implementación provisional** que hay que sustituir:

- Guarda el tablero en un `Dictionary<string, PlacedPieceModel>` con claves `"e4"`.
- `TryApplyMove` sólo valida: casilla de origen ocupada, turno correcto, no capturar pieza propia.
- **Acepta movimientos completamente ilegales** (mover una torre en diagonal, dejar el rey en jaque…).
- No hay enroque, ni al paso, ni jaque, ni mate, ni ahogado, ni tablas.
- El FEN se genera a mano y los derechos de enroque son una heurística (rey y torre en casa).
- No hay parseo de FEN de vuelta.
- Sólo acepta notación larga (`e2e4`), no SAN.

La Fase 2 del `PLAN.md` lo reemplaza por `PositionModel` + `FenSerializer`, y la Fase 3
añade el generador legal. **`GameSessionModel` pasará a envolver `PositionModel`.**

---

## 5. Entorno y comandos

```powershell
# Compilar
dotnet build ChessManagement.sln

# Tests
dotnet test ChessSDK.UnitTests\ChessSDK.UnitTests.csproj

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

2. **Conversiones implícitas ambiguas**
   `FileModel` y `RankModel` convierten implícitamente a `char` **y** a `string`.
   `builder.Append(rank)` no compila (CS0121). Hay que usar `rank.ToString()`.
   La Fase 1 del plan propone eliminar una de las dos conversiones.

3. **Nunca escribir en `Console.Out`** en el proyecto MCP: rompe el protocolo.
   Los logs ya van a stderr en `Program.cs`.

4. **Caracteres Unicode de recuadro** (Fase 6, `draw_board`): hará falta
   `Console.OutputEncoding = Encoding.UTF8` en `Program.cs`.

5. Los value objects **no tienen `Equals`/`GetHashCode`**. Las comparaciones se hacen hoy con
   `ReferenceEquals`. Es lo primero que arregla la Fase 1.

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

- Rama `main`, con cambios pendientes de commitear por el usuario.
- Entre ellos, los renombrados de los 4 archivos movidos de `ChessSDK.Mcp` a `ChessSDK`.

---

## 10. Por dónde continuar

Siguiente paso: **Fase 1 del `PLAN.md`** (`Equals`/`GetHashCode` en los value objects),
porque todo lo demás depende de ello.

Ruta crítica:

```
Fase 1 igualdad → Fase 2 PositionModel+FEN → Fase 3 MoveGenerator+perft ← HITO
     → Fase 4 SAN/PGN → Fase 5 migrar MCP + get_legal_moves
     → Fase 6 draw_board → Fase 7 tests integración → Fase 8 Ollama
```

El hito que valida el proyecto es **perft correcto en Fase 3**: 20 / 400 / 8.902 / 197.281
nodos a profundidad 1-4 desde la posición inicial. Si perft no cuadra, el generador está mal
y el MCP dará movimientos ilegales por buenos.

