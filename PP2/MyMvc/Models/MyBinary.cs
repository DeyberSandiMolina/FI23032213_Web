
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
public class MyBinary
{

    [Required(ErrorMessage = "El valor de 'a' es requerido")]
        [CustomValidation(typeof(MyBinary), nameof(ValidateBinaryString))]
        public string a { get; set; }

        [Required(ErrorMessage = "El valor de 'b' es requerido")]
        [CustomValidation(typeof(MyBinary), nameof(ValidateBinaryString))]
        public string b { get; set; }

        public static ValidationResult ValidateBinaryString(string value, ValidationContext context)
        {
            if (string.IsNullOrEmpty(value))
                return new ValidationResult("El valor no puede estar vacío.");

            if (!Regex.IsMatch(value, "^[01]+$"))
                return new ValidationResult("El valor solo puede contener 0 y 1.");

            if (value.Length > 8)
                return new ValidationResult("El valor no puede tener más de 8 caracteres.");

            if (value.Length % 2 != 0)
                return new ValidationResult("La longitud debe ser múltiplo de 2 (2, 4, 6 u 8).");

            return ValidationResult.Success;
        }
    public string? Binary { get; set; }
    public string? Octal { get; set; }
    public string? Decimal { get; set; }
    public string? Hex { get; set; }
    /*public SolutionResult A { get; set; }
    public SolutionResult B { get; set; }
    public SolutionResult And { get; set; }
    public SolutionResult Or { get; set; }
    public SolutionResult Xor { get; set; }
    public SolutionResult Sum { get; set; }
    public SolutionResult Product { get; set; }*/
}