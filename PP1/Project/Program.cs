
using System;
class Program
{
    //Se implementa la función para sumar los primeros n números naturales de manera recursiva usando la fórmula matemática.
    static int SumFor(int n)
    {
        return n * (n + 1) / 2;
    }

    static int SumRec(int n)
{
    return n > 1 ? n + SumRec(n - 1) : n;
}

    //Se implementa la función para sumar los primeros n números naturales de manera iterativa usando la fórmula matemática a sumRec.
    static int SumIte(int n)
    {
        int sum = 0;
        for (int i = 1; i <= n; i++)
        {
            sum += i;
        }
        return sum;
    }

    static void Main(string[] args)
    {
        const int maximo = int.MaxValue;

        //Ascendente SumFor
        int ultimoValorValidoFor = 0;
        for (int n = 1; n < maximo; n++)
        {
            int suma = SumFor(n);
            if (suma <= 0)
            {
                ultimoValorValidoFor = n - 1;
                int sumaValida = SumFor(ultimoValorValidoFor);
                Console.WriteLine($"\nSumFor Ascendente: From 1 to Max → n: {ultimoValorValidoFor} → sum: {sumaValida}");
                break;
            }
        }

        //Descendente SumFor
        for (int n = maximo; n >= 1; n--)
        {
            int suma = SumFor(n);
            if (suma > 0)
            {
                Console.WriteLine($"\nSumFor Descendente: From Max to 1 → n: {n} → sum: {suma}");
                break;
            }
        }

        //Ascendente SumIte
        int ultimoValorValidoIte = 0;
        for (int n = 1; n < maximo; n++)
        {
            int suma = SumIte(n);
            if (suma <= 0)
            {
                ultimoValorValidoIte = n - 1;
                int sumaValida = SumIte(ultimoValorValidoIte);
                Console.WriteLine($"\nSumIte Ascendente: From 1 to Max → n: {ultimoValorValidoIte} → sum: {sumaValida}");
                break;
            }
        }

        //Descendente SumIte
        for (int n = int.MaxValue; n >= 1; n--)
        {
            int suma = SumIte(n);
            if (suma > 0)
            {
                Console.WriteLine($"\nSumIte Descendente: From Max to 1 → n: {n} → sum: {suma}");
                break;
            }
        }
    }
}