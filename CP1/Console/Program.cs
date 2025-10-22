public class Numbers
{
    private static readonly double N = 25;

    public static double Formula(double z)
    {
        return Round((z + Math.Sqrt(4 + Math.Pow(z, 2))) / 2);
    }

    public static double Recursive(double z)
    {
        return Round(Recursive(z, N) / Recursive(z, N - 1));
    }

    public static double Iterative(double z)
    {
        return Round(Iterative(z, N) / Iterative(z, N - 1));
    }

    private static double Recursive(double z, double n)
    {

        if (n == 0 || n == 1)
            return 1;

        double result = z * Recursive(z, n - 1) + Recursive(z, n - 2);

        double Decimal = result % 1;
        double Entero = result - Decimal;

        if (Decimal >= 0.5)
            result = Entero + 1;
        else if (Decimal <= -0.5)
            result = Entero - 1;
        else
            result = Entero;

        return result;
    }


     private static double Iterative(double z, double n)
   {
        if (n == 0 || n == 1)
            return 1;

        double a = 1;
        double b = 1;
        double result = 0;

        for (double i = 2; i <= n; i++)
        {
            result = z * b + a;

            double Decimal = result % 1;
            double Entero = result - Decimal;

            if (Decimal >= 0.5)
                result = Entero + 1;
            else if (Decimal <= -0.5)
                result = Entero - 1;
            else
                result = Entero;

            a = b;
            b = result;
        }

        return result;


      


    }

    private static double Round(double value)
    {
        return Math.Round(value, 10);
    }

    public static void Main(String[] args)
    {
        String[] metallics = [
            "Platinum", // [0]
            "Golden", // [1]
            "Silver", // [2]
            "Bronze", // [3]
            "Copper", // [4]
            "Nickel", // [5]
            "Aluminum", // [6]
            "Iron", // [7]
            "Tin", // [8]
            "Lead", // [9]
        ];
        for (var z = 0; z < metallics.Length; z++)
        {
            Console.WriteLine("\n[" + z + "] " + metallics[z]);
            Console.WriteLine(" ↳ formula(" + z + ")   ≈ " + Formula(z));
            Console.WriteLine(" ↳ recursive(" + z + ") ≈ " + Recursive(z));
            Console.WriteLine(" ↳ iterative(" + z + ") ≈ " + Iterative(z));
        }
    }
}
