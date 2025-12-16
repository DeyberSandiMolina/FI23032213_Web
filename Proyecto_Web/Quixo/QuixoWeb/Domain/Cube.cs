namespace QuixoWeb.Domain
{
    public enum CubeSymbol { Neutral, Circle, Cross }

    public enum CubeOrientation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public class Cube
    {
        public CubeSymbol Symbol { get; set; }
        public CubeOrientation Orientation { get; set; }

        public Cube()
        {
            Symbol = CubeSymbol.Neutral;
            Orientation = CubeOrientation.Up;
        }

        public Cube Clone()
        {
            return new Cube
            {
                Symbol = this.Symbol,
                Orientation = this.Orientation
            };
        }

    }
}
