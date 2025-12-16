namespace QuixoWeb.Models
{
public class BoardState
{
    public int BoardStateId { get; set; }

    public int GameId_FK { get; set; }
    public int MoveId { get; set; }

    public string StateJson { get; set; } = string.Empty;

    public Game? Game { get; set; }
    public Move? Move { get; set; }
}



}
