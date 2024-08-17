namespace Minesweeper.Models;

public class Game(
    Guid gameId,
    int width,
    int height,
    int minesCount,
    bool completed,
    string[,] field,
    bool[,] revealed,
    bool[,] mines)
{
    public Guid GameId { get; init; } = gameId;
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
    public int MinesCount { get; set; } = minesCount;
    public bool Completed { get; set; } = completed;
    public string[,] Field { get; set; } = field;
    public bool[,] Revealed { get; set; } = revealed;
    public bool[,] Mines { get; set; } = mines;
}