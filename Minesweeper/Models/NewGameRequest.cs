namespace Minesweeper.Models;

public class NewGameRequest
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int MinesCount { get; set; }
}