

namespace QuixoWeb.Application.DTOs
{
    public class MoveRequestDto
    {
        public int TakeRow { get; set; }
        public int TakeCol { get; set; }

        public int PlaceRow { get; set; }
        public int PlaceCol { get; set; }
        public int? Orientation { get; set; }
    }
}
