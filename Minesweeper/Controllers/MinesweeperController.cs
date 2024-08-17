using Microsoft.AspNetCore.Mvc;
using Minesweeper.Models;

namespace Minesweeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MinesweeperController(IMinesweeperService minesweeperService) : ControllerBase
{
    private readonly IMinesweeperService _minesweeperService = minesweeperService;

    [HttpPost("new")]
    public ActionResult<GameInfoResponse> CreateGame([FromBody] NewGameRequest request)
    {
        try
        {
            var game = _minesweeperService.CreateGame(request.Width, request.Height, request.MinesCount);
            return Ok(game);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse { Error = ex.Message });
        }
    }

    [HttpPost("turn")]
    public ActionResult<GameInfoResponse> MakeTurn([FromBody] GameTurnRequest request)
    {
        try
        {
            var game = _minesweeperService.MakeTurn(request.GameId, request.Row, request.Col);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Error = ex.Message });
        }
    }
}
