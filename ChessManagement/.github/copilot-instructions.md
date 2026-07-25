El orden en C# es:

A partir de ahora, cuando generes clases C#, sigue este orden:

1. Campos
    - public const
    - public static
    - public readonly

    - internal const
    - internal static
    - internal readonly

    - protected const
    - protected static
    - protected readonly

    - private const
    - private static
    - private readonly

2. Constructores
    - public
    - internal
    - protected
    - private

3. Propiedades
    - public
    - internal
    - protected
    - private

4. Métodos static
    - public
    - internal
    - protected
    - private

5. Métodos abstract

6. Métodos de instancia
    - public
    - internal
    - protected
    - private

7. Overrides
    - public override
    - internal override
    - protected override

## Nomenclatura de archivos y clases

Todos los archivos y sus clases deben terminar con un sufijo que indique **qué son**. El nombre del archivo debe
coincidir siempre con el nombre del tipo que contiene.

Sufijos a utilizar:

| Sufijo                            | Uso                                                                   |
|-----------------------------------|-----------------------------------------------------------------------|
| `Model`                           | Entidades y objetos de dominio o de estado. `BoardModel`, `MoveModel` |
| `Service`                         | Lógica de aplicación, orquestación y almacenes. `GameStoreService`    |
| `Repository`                      | Acceso a datos persistidos                                            |
| `Db` / `DbContext`                | Contextos de base de datos                                            |
| `Enum`                            | Enumeraciones                                                         |
| `Formatter`                       | Conversores de formato de salida. `LanFormatter`                      |
| `Parser`                          | Conversores de formato de entrada                                     |
| `Serializer`                      | Conversión bidireccional. `FenSerializer`                             |
| `Generator`                       | Generación de conjuntos derivados. `MoveGenerator`                    |
| `Validator`                       | Reglas de validación                                                  |
| `Factory`                         | Creación de objetos                                                   |
| `Extensions`                      | Métodos de extensión                                                  |
| `Options` / `Settings`            | Configuración                                                         |
| `ViewModel`                       | Modelos de vista de MAUI                                              |
| `Tools` / `Resources` / `Prompts` | Contenedores de primitivas MCP                                        |
| `Tests`                           | Clases de pruebas unitarias. `BoardModelTests`                        |
| `IntegrationTests`                | Clases de pruebas de integración. `McpServerIntegrationTests`         |

Reglas adicionales:

- Las interfaces mantienen el prefijo `I` **y** el sufijo correspondiente: `IGameStoreService`,
  `IMoveNotationFormatter`.
- No se usan sufijos genéricos sin significado como `Helper`, `Manager`, `Util` o `Common`.
- Un archivo contiene un único tipo público.
- La carpeta debe concordar con el sufijo: los `*Model` viven en `Models/`, los `*Service` en `Services/`, etc.
- Única excepción: `Program.cs`, punto de entrada de la aplicación.

## Arquitectura: dónde vive la lógica

`ChessSDK` es el **único** proyecto que contiene lógica de ajedrez. Es reutilizable desde
cualquier salida futura: servidor MCP, app MAUI, API web, CLI...

Los proyectos de salida (`ChessSDK.Mcp`, y en el futuro app o web) son **adaptadores finos**.
Sólo pueden contener:

- Registro de dependencias y arranque (`Program.cs`).
- Declaración de primitivas del protocolo o de la UI (`*Tools`, `*Resources`, `*Prompts`, `*ViewModel`, controladores).
- Traducción entre los tipos del SDK y el formato de entrada/salida del canal.

Está **prohibido** en un proyecto de salida:

- Reglas de ajedrez, validación de movimientos o estado de partida.
- Generación de FEN, PGN, SAN o dibujo de tableros.
- Cualquier `if` sobre lógica de dominio que pudiera necesitarse desde otra salida.

Regla práctica: si el código haría falta igualmente en una app MAUI, va en `ChessSDK`.

