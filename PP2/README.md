# README

## Información del Estudiante

**Nombre:** Deyber Sandi Molina  
**Carné:** FI23032213

## Comandos de dotnet utilizados (CLI)

- `dotnet new sln -n MySolution`
- `dotnet new mvc -n MyMvc`
- `dotnet sln add MyMvc\`

## Prompts (consultas y respuestas) de los chatbots de IA (Copilot, Gemini, ChatGPT, etc.) utilizados

- **Pregunta:** ¿Cómo cambiar para que al correr el código, lo primero que se vea sea el formulario y no el Index?
  
  **Respuesta:** Por defecto en un proyecto ASP.NET Core MVC, lo primero que se carga es el `HomeController → Index()`. Para mostrar primero el formulario (`MyForm`) al entrar a `http://localhost:5000/`, se puede cambiar la ruta por defecto en `Program.cs` de la siguiente manera:

  ```csharp
  app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=MyForm}/{id?}");
  ```

## Respuestas a preguntas

- **Pregunta:** ¿Cuál es el número que resulta al multiplicar si se introducen los valores máximos permitidos en `a` y `b`? Indíquelo en todas las bases (binaria, octal, decimal y hexadecimal).

  **Valores:**
  - a = 11111111
  - b = 11111111

  **Resultados:**
  - Binario: 1111111000000001
  - Octal: 177001
  - Decimal: 65025
  - Hexadecimal: FE01

- **Pregunta:** ¿Es posible hacer las operaciones en otra capa? Si sí, ¿en cuál sería?

  **Respuesta:** Los métodos también se pudieron haber realizado directamente en `MyBinary`.
