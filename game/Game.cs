namespace Chess;

/// <summary>
/// Represents a game of chess, which has a board and two players.
/// </summary>
/// <param name="playerWhite">The player who is controlling the white pieces.</param>
/// <param name="playerBlack">The player who is controlling the black pieces.</param>
class Game(Player playerWhite, Player playerBlack)
{
    private readonly Board Board = new();
    private readonly Player PlayerWhite = playerWhite;
    private readonly Player PlayerBlack = playerBlack;
    private int MoveCount = 1;

    /// <summary>
    /// Game loop. Continually prompts white and black (in that order) to make moves until the game meets an ending condition.
    /// </summary>
    public void Play()
    {
        Console.WriteLine(Board.StateBoard());

        while (true)
        {
            if (Board.IsCheckmate(Constants.Color.White))
            {
                Console.WriteLine("Black checkmates white!\n");
                break;
            }

            if (Board.IsDraw())
            {
                Console.WriteLine("Draw!\n");
                break;
            }

            if (Board.IsStalemate(Constants.Color.White))
            {
                Console.WriteLine("Stalemate.\n");
                break;
            }

            if (Board.IsCheck(Constants.Color.White))
            {
                Console.WriteLine("White is in check!\n");
            }

            Console.WriteLine($"Turn {MoveCount}, white to move...\n");

            Move? move = PlayerWhite.PromptMove(Board);

            if (move == null)
            {
                Console.WriteLine("White forfeits.\n");
                break;
            }

            Console.WriteLine($"White has moved {move.StateMove()}.\n");

            Board.MovePiece(move);
            MoveCount++;

            Console.WriteLine(Board.StateBoard(move));

            if (Board.IsCheckmate(Constants.Color.Black))
            {
                Console.WriteLine("White checkmates black!\n");
                break;
            }

            if (Board.IsDraw())
            {
                Console.WriteLine("Draw!\n");
                break;
            }

            if (Board.IsStalemate(Constants.Color.Black))
            {
                Console.WriteLine("Stalemate.\n");
                break;
            }

            if (Board.IsCheck(Constants.Color.Black))
            {
                Console.WriteLine("Black is in check!\n");
            }

            Console.WriteLine($"Turn {MoveCount}, black to move...\n");

            move = PlayerBlack.PromptMove(Board);

            if (move == null)
            {
                Console.WriteLine("Black forfeits.\n");
                break;
            }

            Console.WriteLine($"Black has moved {move.StateMove()}.\n");

            Board.MovePiece(move);
            MoveCount++;

            Console.WriteLine(Board.StateBoard(move));
        }
    }
}