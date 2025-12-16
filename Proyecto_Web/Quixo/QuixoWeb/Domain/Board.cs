namespace QuixoWeb.Domain
{
    public class Board
    {
        public Cube[,] Grid { get; set; } = new Cube[5, 5];

        public Board()
        {
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    Grid[r, c] = new Cube();
        }

        public Cube Get(int r, int c) => Grid[r, c];

        public bool IsPerimeter(int r, int c)
        {
            return r == 0 || r == 4 || c == 0 || c == 4;
        }

        public Board Clone()
        {
            var copy = new Board();

            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    var original = this.Grid[r, c];

                    copy.Grid[r, c] = new Cube
                    {
                        Symbol = original.Symbol,
                        Orientation = original.Orientation
                    };
                }
            }

            return copy;
        }

        

    }
}
