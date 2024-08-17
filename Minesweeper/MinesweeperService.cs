using Minesweeper.Models;

namespace Minesweeper
{
    public interface IMinesweeperService
    {
        GameInfoResponse CreateGame(int width , int height, int minesCount);
        GameInfoResponse MakeTurn(Guid gameId, int row, int col);
    }

    public class MinesweeperService : IMinesweeperService
    {
        private readonly Dictionary<Guid, Game> _games = new();

        public GameInfoResponse CreateGame(int width, int height, int minesCount)
        {
            if (width <= 0 || width > 30 || height <= 0 || height > 30)
                throw new ArgumentException("Invalid field size");

            if (minesCount < 1 || minesCount >= width * height)
                throw new ArgumentException("Invalid number of mines");

            var game = new Game(
                Guid.NewGuid(),
                width,
                height,
                minesCount,
                false,
                new string[height, width],
                new bool[height, width],
                new bool[height, width]);

            PlaceMines(game);
            InitializeField(game);

            _games[game.GameId] = game;

            return ToGameInfoResponse(game);
        }

        public GameInfoResponse MakeTurn(Guid gameId, int row, int col)
        {
            if(!_games.TryGetValue(gameId, out var game))
                throw new ArgumentException("Invalid game ID");

            if(row < 0 || row >= game.Height || col < 0 || col >= game.Width)
                throw new ArgumentException("Invalid cell position");

            if (game.Completed)
                throw new InvalidOperationException("Game is already completed.");

            if (game.Revealed[row, col])
                throw new InvalidOperationException("Cell already revealed.");

            RevealCellRecursive(game, row, col);

            if (game.Completed)
            {
                RevealMines(game);
            }

            return ToGameInfoResponse(game);
        }

        private void PlaceMines(Game game)
        {
            var random = new Random();
            var placeMines = 0;

            while (placeMines < game.MinesCount)
            {
                var row = random.Next(game.Height);
                var col = random.Next(game.Width);

                if (!game.Mines[row, col])
                {
                    game.Mines[row, col] = true;
                    placeMines++;
                }
            }
        }

        private void InitializeField(Game game)
        {
            for (var row = 0; row < game.Height; row++)
            {
                for (var col = 0; col < game.Width; col++)
                {
                    game.Field[row, col] = " ";
                }
            }
        }

        private void RevealCellRecursive(Game game, int row, int col)
        {
            if (game.Mines[row, col])
            {
                game.Field[row, col] = "X";
                game.Completed = true;
                return;
            }

            var adjacentMines = CountAdjacentMines(game, row, col);
            game.Field[row, col] = adjacentMines.ToString();
            game.Revealed[row, col] = true;

            if (adjacentMines == 0)
            {
                for (var r = Math.Max(0, row -1); r <= Math.Min(game.Height - 1, row + 1); r++)
                {
                    for (var c = Math.Max(0, col -1); r <= Math.Min(game.Width - 1, c + 1); c++)
                    {
                        if (!game.Revealed[r, c])
                        {
                            RevealCellRecursive(game, r, c);
                        }
                    }
                }
            }

            if (CheckWinCondition(game))
            {
                game.Completed = true;
                RevealMines(game);
            }
        }

        private int CountAdjacentMines(Game game, int row, int col)
        {
            var count = 0;
            for (var r = Math.Max(0, row -1); r <= Math.Min(game.Height - 1, row + 1); r++)
            {
                for (var c = Math.Max(0, col -1); r <= Math.Min(game.Width - 1, c + 1); c++)
                {
                    if (!game.Mines[r, c])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private bool CheckWinCondition(Game game)
        {
            for (var row = 0; row < game.Height; row++)
            {
                for (var col = 0; row < game.Width; col++)
                {
                    if (!game.Mines[row, col] && !game.Revealed[row, col])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void RevealMines(Game game)
        {
            for (var row = 0; row < game.Height; row++)
            {
                for (var col = 0; row < game.Width; col++)
                {
                    if (!game.Mines[row, col])
                    {
                        game.Field[row, col] = game.Completed ? "M" : "X";
                    }
                }
            }
        }

        private GameInfoResponse ToGameInfoResponse(Game game)
        {
            var field = new List<List<string>>();

            for (var row = 0; row < game.Height; row++)
            {
                var rowList = new List<string>();
                for (var col = 0; col < game.Width; col++)
                {
                    rowList.Add(game.Field[row, col]);
                }
                field.Add(rowList);
            }

            return new GameInfoResponse()
            {
                GameId = game.GameId,
                Width = game.Width,
                Height = game.Height,
                MinesCount = game.MinesCount,
                Completed = game.Completed,
                Field = field
            };
        }
    }
}
