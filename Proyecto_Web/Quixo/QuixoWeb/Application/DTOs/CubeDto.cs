namespace QuixoWeb.Application.DTOs
{
    public class CubeDto
    {
        // Guardamos el symbol como entero para que JSON lo serialice fácilmente
        public int Symbol { get; set; }           // 0 = Neutral, 1 = Circle, 2 = Cross
        public int Orientation { get; set; }      // 0=Up,1=Right,2=Down,3=Left
    }
}
