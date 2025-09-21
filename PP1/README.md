# Tarea: Suma Recursiva e Iterativa

**Nombre:** Deyber Sandi Molina  
**Carné:** FI23032213  

---

## Comandos utilizados


dotnet new sln --name Solution
dotnet new console --output Project
dotnet sln add Project
dotnet build
dotnet run


---

## Consultas realizadas a IA (ChatGPT)

### Pregunta
```
SumRec(1) = 1
SumRec(n) = n + SumRec(n - 1)

De manera iterativa
```


### Respuesta
Podemos convertir esa definición recursiva a una versión iterativa usando un bucle que acumule la suma.

Pseudocódigo

función SumRecIter(n):
    suma ← 0
    para i ← 1 hasta n:
        suma ← suma + i
    retornar suma


**Ejemplo en Python**

python
def SumRecIter(n):
    suma = 0
    for i in range(1, n + 1):
        suma += i
    return suma

print(SumRecIter(5))  # Salida: 15 (1+2+3+4+5)


**Ejemplo en Java**
java
int SumRecIter(int n) {
    int suma = 0;
    for (int i = 1; i <= n; i++) {
        suma += i;
    }
    return suma;
}


---

## Pregunta
¿Por qué todos los valores resultantes, tanto de `n` como de `sum`, difieren entre métodos (fórmula e implementación iterativa) y estrategias (ascendente y descendente)?

### Respuesta
La principal diferencia radica en el momento en el que ocurre el **overflow**.  
`SumFor` provoca un overflow más rápido que `SumIte`, ya que este último va sumando de a poco y se acumula más lentamente. Por ende, `SumIte` puede alcanzar un valor más alto que `SumFor` antes de que se produzca un resultado no válido.

---

## Pregunta
¿Qué sucedería si se utilizan las mismas estrategias (ascendente y descendente) pero con el método recursivo de suma (`SumRec`)?

### Respuesta
Al usar la versión recursiva `SumRec`, la **pila de llamadas** puede sobrepasar su límite, generando un error de tipo **Stack Overflow** que detendrá el programa cuando `N` sea muy grande.  
Esto depende del equipo, pero aproximadamente con `N ≥ 45,000` se produciría este error.

---
