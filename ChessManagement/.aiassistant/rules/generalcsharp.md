---
apply: always
---


## Objetivo
El código debe ser claro, mantenible y coherente con las convenciones modernas de C# y .NET MAUI.

## Formato y documentación
- Siempre que sea posible en la indentación usa tabs en lugar de espacios.
- El código, nombres de variables, comentarios y documentación debe ir en inglés.
- No incluir comentarios redundantes (“este método devuelve X”).
- Mantén el código limpio: sin regiones vacías ni comentarios obsoletos.

## Estilo general
- Prefiere **expresiones lambda** y **propiedades autoimplementadas** cuando simplifiquen el código.
- Mantén los **nombres descriptivos** y consistentes con las guías de Microsoft.
- Aplica **principios SOLID** y evita dependencias circulares.
- Usa **async/await** para operaciones asíncronas; evita `Task.Result` o `Wait()`. Los métodos asyncronos deben terminar su nombre en Async.

## Tests
- Los nombres deben usar el patrón Given When Value con camel case separando cada sección por guiones bajos: GivenAScenarioOrObject_WhenDoSomeThing_ThenResultExpected.
- Los test deben realizarse usando el patrón Arrange - Act - Assert
- No mezclar lógica de negocio con mocks o helpers.
- Cada test debe ser independiente y reproducible.