# README

## Información del Estudiante

**Nombre:** Deyber Sandi Molina  
**Carné:** FI23032213

## Comandos de dotnet utilizados (CLI)

- `dotnet new sln -n MySolution`
- `dotnet new webapu -o MinimalApi`
- `dotnet sln add MinimalApi\`

## Prompts (consultas y respuestas) de los chatbots de IA (Copilot, Gemini, ChatGPT, etc.) utilizados

- **Pregunta:** 
  
  **Respuesta:** En una Minimal API con .NET 8.0 (C#), para recorrer un string palabra por palabra usando el espacio como separador, lo más común y directo es usar el método:

  ```csharp
  Split(' ')
    ```

O mejor aún, si quieres evitar espacios vacíos entre palabras (por ejemplo, si hay más de un espacio seguido), usas:

```csharp
Split(' ', StringSplitOptions.RemoveEmptyEntries)
  ```

Ejemplo completo:
```csharp
var text = "This  is   a   test";
var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

foreach (var word in words)
{
    Console.WriteLine(word);
}
  ```
## Respuestas a preguntas

- **Pregunta:** ¿Es posible enviar valores en el Body (por ejemplo, en el Form) del Request de tipo GET?

  **Respuesta**
  - Al menos yo agregué texto usando el metodo 'get' en 'postman' en el form y luego igual funciono para los demás métodos, o tal vez no comprendí la pregunta, pero mi respuesta es un sí.

- **Pregunta:** ¿Qué ventajas y desventajas observa con el Minimal API si se compara con la opción de utilizar Controllers?

  **Respuesta:** 
  - En sí la mayor diferencia que puedo ver es que casi todo se hace desde el program.cs, lo cual puede ser una ventaja para cosas pequeñas, ya que se tiene todo junto, pero si se utilizara para cosas muy grandes, presentaría un gran desorde, a diferencia de usar controllers específicos para cada tarea que se necesite.
