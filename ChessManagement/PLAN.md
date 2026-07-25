# Plan de implementación — ChessManagement MCP

Estado de partida: `ChessSDK` sólo tiene value objects y formatters.
`ChessSDK.Mcp` funciona por stdio con 6 tools, 2 resources y 2 prompts, pero **sin reglas reales**.

Objetivo: que un LLM local no pueda hacer jugadas ilegales porque el servidor MCP se lo impide.

---

## Principio arquitectónico

**Toda la lógica de ajedrez vive en `ChessSDK`.** Los proyectos de salida (`ChessSDK.Mcp`, y
mañana una app MAUI o una web) son adaptadores finos: registran dependencias, declaran las
primitivas del canal y traducen tipos. Nada más.

Cada fase de este plan implementa en `ChessSDK` y sólo expone en el proyecto de salida.
Si una fase necesita añadir un `if` de dominio en `ChessSDK.Mcp`, está mal diseñada.

| Proyecto | Contiene |
|---|---|
| `ChessSDK` | Modelos, reglas, notación, formatters, servicios de partida |
| `ChessSDK.Mcp` | `Program.cs`, `*Tools`, `*Resources`, `*Prompts` |
| Futura app/web | `Program.cs`, `*ViewModel` / controladores |

---

## Convención nueva a añadir a `copilot-instructions.md`

| Sufijo             | Uso                                                         |
|--------------------|-------------------------------------------------------------|
| `Generator`        | Generación de conjuntos de datos derivados. `MoveGenerator` |
| `Serializer`       | Serialización bidireccional. `FenSerializer`                |
| `IntegrationTests` | Pruebas de integración extremo a extremo                    |

---

## Fase 1 — Igualdad en los value objects

**Por qué primero:** sin `Equals`/`GetHashCode` no hay diccionarios de posición, ni comparación de casillas, ni
detección de repetición triple. Todo lo demás se apoya en esto.

### Tareas

1. `FileModel`, `RankModel`: implementar `IEquatable<T>`, `Equals`, `GetHashCode`, `operator ==` / `!=`. Ojo: ya tienen
   conversiones implícitas a `char` y `string`; añadir operadores puede crear ambigüedades. Verificar que
   `BoardModelTests` sigue compilando.
2. `CoordinateModel`: igualdad por `File` + `Rank`. Añadir `Index` (0-63) para indexar arrays.
3. `PieceModel`, `GameColorModel`: son singletons; documentar que la igualdad es por referencia y sellar el patrón
   (constructor privado + instancias estáticas). Añadir `Symbol` y `Value`
   (valor material: peón 1, caballo 3, ...).
4. Corregir el olor detectado: `FileModel` y `RankModel` tienen conversión implícita a `char`
   **y** a `string`, lo que ya provocó un error de ambigüedad en `StringBuilder.Append`. Propuesta: dejar sólo la
   conversión a `char` y un `ToString()` explícito.

### Criterio de aceptación

- `CoordinateModel.Create(FileModel.E, RankModel.R4).Equals("e4")` es `true`.
- Un `HashSet<CoordinateModel>` con 64 casillas distintas tiene `Count == 64`.
- Tests: `CoordinateModelTests`, `FileModelTests`, `RankModelTests`, `PieceModelTests`.

---

## Fase 2 — Estado de posición y FEN

### Tareas

1. **`Models/Boards/PositionModel.cs`**
    - `PlacedPieceModel[] squares` de 64 posiciones (mover el tipo desde `ChessSDK.Mcp.Models`).
    - `SideToMove`, `CastlingRights`, `EnPassantTarget`, `HalfMoveClock`, `FullMoveNumber`.
    - `PositionModel.StartingPosition` estático.
    - `Clone()` y aplicación inmutable de movimientos: `Apply(MoveModel) → PositionModel`.
2. **`Models/Boards/CastlingRightsModel.cs`**: flags `WhiteKingSide`, `WhiteQueenSide`, `BlackKingSide`,
   `BlackQueenSide`.
3. **`Notation/FenSerializer.cs`**: `Serialize(PositionModel)` y `Deserialize(string)`.
4. Retirar la generación de FEN artesanal de `GameSessionModel`.

### Criterio de aceptación

- Round-trip: `Deserialize(Serialize(p))` equivale a `p` para 20 FEN conocidos (inicial, Kiwipete, posiciones con al
  paso y sin derechos de enroque).
- FEN inicial exacto: `rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1`.

---

## Fase 3 — Generador de movimientos legales

**El núcleo del proyecto.** Sin esto el MCP no aporta garantías.

### Tareas

1. **`Rules/MoveGenerator.cs`**
    - Pseudo-legales por pieza: peón (avance simple/doble, capturas, al paso, promoción), caballo, alfil, torre, dama,
      rey.
    - Enroque: casillas vacías, rey no en jaque, casillas intermedias no atacadas, derechos vigentes.
2. **`Rules/AttackDetector.cs`**: `IsSquareAttacked(PositionModel, CoordinateModel, GameColorModel)`.
3. **`Rules/LegalityValidator.cs`**: filtra pseudo-legales simulando el movimiento y descartando los que dejan al rey
   propio en jaque.
4. **`Rules/GameResultEvaluator.cs`**: jaque, mate, ahogado, material insuficiente, repetición triple (requiere hash de
   posición de la Fase 1), regla de 50 movimientos.

### Criterio de aceptación — perft

Sin esto no se continúa. Valores de referencia desde la posición inicial:

| Profundidad | Nodos     |
|-------------|-----------|
| 1           | 20        |
| 2           | 400       |
| 3           | 8.902     |
| 4           | 197.281   |
| 5           | 4.865.609 |

Más `Kiwipete` (`r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -`):
depth 1 = 48, depth 2 = 2.039, depth 3 = 97.862.

Tests en `PerftTests`, con los de profundidad ≥ 4 marcados como categoría `Slow`.

---

## Fase 4 — Notación SAN completa

### Tareas

1. **`Notation/SanParser.cs`**: de `Nbd2`, `exd5`, `O-O`, `e8=Q+` a `MoveModel`, apoyándose en la lista de legales para
   resolver ambigüedades.
2. Refactor de los formatters: `EnglishSanFormatter`, `SpanishSanFormatter` y `FigurineSanFormatter`
   comparten el 90% del código. Extraer `SanFormatterBase` con un mapa de letras inyectable.
3. Añadir a los formatters: desambiguación, enroque, `+` (jaque) y `#` (mate).
4. **`Notation/PgnFormatter.cs`**: exportar la partida completa con cabeceras `[Event]`, `[Date]`, `[Result]`.

### Criterio de aceptación

- Round-trip SAN: parsear y volver a formatear 100 jugadas de una partida real da el mismo texto.
- La ambigüedad `Nbd2` se resuelve correctamente con dos caballos que alcanzan `d2`.

---

## Fase 5 — Migración del servidor MCP

### Tareas

1. `GameSessionModel` deja de tener diccionario propio: pasa a envolver
   `PositionModel` + historial + resultado, y expone `Undo()`.
2. Nueva tool **`get_legal_moves`**
    - Parámetros: `gameId`, `from` opcional (para filtrar por casilla de origen).
    - Devuelve la lista en notación larga **y** SAN, agrupada por pieza.
    - `ReadOnly = true`.
3. `make_move` valida contra la lista de legales. Mensaje de error explícito y accionable:
   `"g1f3 no es legal. Movimientos legales del caballo de g1: g1f3, g1h3."`
4. `get_position` añade: `enCheck`, `result` (`in_progress`, `checkmate`, `stalemate`, `draw`), `legalMoveCount`.
5. Nueva tool **`undo_move`**.
6. Nueva tool **`draw_board`** (ver Fase 6).
7. Endurecer `ChessPrompts.PlayChess`: obligar a `get_legal_moves` antes de cada `make_move`.

---

## Fase 6 — Tool `draw_board`

Tablero ASCII con líneas de recuadro y nomenclatura traducible.

### Firma

```csharp
[McpServerTool(Name = "draw_board", ReadOnly = true)]
public string DrawBoard(string gameId, string language = "es", string perspective = "white")
```

- `language`: `es` (TCADRP), `en` (RNBQKP), `figurine` (♔♕♖♗♘♙).
- `perspective`: `white` o `black` (invierte filas y columnas).
- Mayúsculas para blancas, minúsculas para negras; en modo figurine se usan las piezas blancas y negras propias de
  Unicode.

### Salida esperada (español, perspectiva de blancas)

```
    a   b   c   d   e   f   g   h
  ┌───┬───┬───┬───┬───┬───┬───┬───┐
8 │ t │ c │ a │ d │ r │ a │ c │ t │ 8
  ├───┼───┼───┼───┼───┼───┼───┼───┤
7 │ p │ p │ p │ p │ p │ p │ p │ p │ 7
  ├───┼───┼───┼───┼───┼───┼───┼───┤
6 │   │   │   │   │   │   │   │   │ 6
  ├───┼───┼───┼───┼───┼───┼───┼───┤
5 │   │   │   │   │   │   │   │   │ 5
  ├───┼───┼───┼───┼───┼───┼───┼───┤
4 │   │   │   │   │   │   │   │   │ 4
  ├───┼───┼───┼───┼───┼───┼───┼───┤
3 │   │   │   │   │   │   │   │   │ 3
  ├───┼───┼───┼───┼───┼───┼───┼───┤
2 │ P │ P │ P │ P │ P │ P │ P │ P │ 2
  ├───┼───┼───┼───┼───┼───┼───┼───┤
1 │ T │ C │ A │ D │ R │ A │ C │ T │ 1
  └───┴───┴───┴───┴───┴───┴───┴───┘
    a   b   c   d   e   f   g   h
```

### Tareas

1. **`Formatters/BoardAsciiFormatter.cs`** en `ChessSDK` (es lógica de presentación de dominio, reutilizable desde
   MAUI).
2. **`Formatters/PieceLetterProvider.cs`**: mapa de letras por idioma, reutilizando los mapas que ya existen en
   `SpanishSanFormatter` / `EnglishSanFormatter` / `FigurineSanFormatter`.
3. Tool `draw_board` en `ChessGameTools` que delega en el formatter.
4. Actualizar el resource `chess://game/{gameId}/board` para usar el nuevo dibujo.

### Criterio de aceptación

- El dibujo de la posición inicial coincide carácter a carácter con el fixture esperado.
- `perspective=black` coloca la fila 1 arriba y la columna `h` a la izquierda.
- Los tres idiomas producen las letras correctas.
- Aviso: los caracteres de recuadro son Unicode; el servidor debe escribir en UTF-8
  (`Console.OutputEncoding = Encoding.UTF8` en `Program.cs`).

---

## Fase 7 — Tests de integración

Proyecto nuevo **`ChessSDK.Mcp.IntegrationTests`** (MSTest + AwesomeAssertions).

Levanta el servidor MCP **como proceso real** y habla con él por stdio usando el cliente oficial (`McpClient` +
`StdioClientTransport`), sin mocks. Es la prueba de que el protocolo, la serialización y las reglas funcionan juntos.

### Escenarios

| #  | Escenario             | Verifica                                                                |
|----|-----------------------|-------------------------------------------------------------------------|
| 1  | Handshake             | `initialize` devuelve capabilities de tools, resources y prompts        |
| 2  | Inventario            | `tools/list` devuelve las 9 tools con su `inputSchema`                  |
| 3  | Partida nueva         | `new_game` devuelve el FEN inicial exacto                               |
| 4  | Movimiento legal      | `make_move e2e4` cambia el turno y el FEN                               |
| 5  | **Movimiento ilegal** | `make_move e2e5` es rechazado y el FEN **no** cambia                    |
| 6  | **Pieza inexistente** | `make_move e3e4` devuelve error explicativo                             |
| 7  | **Turno equivocado**  | Negras intentando mover en el turno de blancas → error                  |
| 8  | Legales               | `get_legal_moves` devuelve 20 en la posición inicial                    |
| 9  | Mate del pastor       | Secuencia completa → `result = checkmate`                               |
| 10 | Mate del loco         | `f2f3 e7e5 g2g4 d8h4` → mate en 2                                       |
| 11 | Ahogado               | Posición preparada → `result = stalemate`                               |
| 12 | Enroque               | `O-O` mueve rey y torre a la vez                                        |
| 13 | Al paso               | Captura al paso válida sólo en la jugada inmediata                      |
| 14 | Promoción             | `e7e8q` coloca una dama                                                 |
| 15 | Undo                  | `undo_move` restaura el FEN anterior                                    |
| 16 | Resources             | `resources/read` de `chess://game/{id}/fen` coincide con `get_position` |
| 17 | Prompts               | `prompts/get` de `play_chess` devuelve mensajes no vacíos               |
| 18 | Dibujo                | `draw_board` en es/en/figurine y ambas perspectivas                     |
| 19 | Aislamiento           | Dos partidas simultáneas no se interfieren                              |
| 20 | Robustez              | `gameId` inexistente → error controlado, no excepción de protocolo      |

### Infraestructura

- `McpServerFixture`: arranca `ChessSDK.Mcp` una vez por clase de test y lo mata al terminar.
- Compilar el servidor antes de los tests (`ProjectReference` al csproj para forzar el orden).
- Timeout por llamada de 10 s para que un cuelgue no bloquee la suite.

### Criterio de aceptación

Los 20 escenarios en verde. **Los escenarios 5, 6, 7 y 10 son los que demuestran la tesis del proyecto**: el modelo no
puede alucinar porque el servidor le corta.

---

## Fase 8 — Cliente local con Ollama

1. `ollama pull qwen3:8b`.
2. Proyecto `ChessSDK.McpClient`: consola + `OllamaSharp` (`IChatClient`) +
   `Microsoft.Extensions.AI` con `UseFunctionInvocation()` + cliente MCP.
3. Las tools MCP se exponen como `AIFunction` automáticamente.
4. Bucle de chat por consola para jugar contra el modelo.
5. Métrica a registrar: **número de intentos ilegales por partida**. Debería tender a 0 conforme el prompt obligue a
   `get_legal_moves`.

---

## Orden de ejecución y dependencias

```
Fase 1 (igualdad)
   └─> Fase 2 (PositionModel + FEN)
          └─> Fase 3 (MoveGenerator + perft)   ← hito crítico
                 ├─> Fase 4 (SAN/PGN)
                 └─> Fase 5 (migración MCP)
                        ├─> Fase 6 (draw_board)
                        └─> Fase 7 (tests de integración)
                               └─> Fase 8 (Ollama)
```

## Deuda técnica pendiente

- Quitar el pin de `System.Text.Json 10.0.10` al actualizar al runtime .NET 10 final.
- `InMemoryGameStoreService` pierde las partidas al reiniciar; valorar `GameRepository`.
- `MatchModel` y `PlayerModel` están huérfanos: o se integran en `GameSessionModel` o se eliminan.

