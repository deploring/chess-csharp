namespace Chess;

class ComputerPlayer(Constants.Color color, int depth) : Player(color)
{
    /// <summary>
    /// Heuristic evaluation coefficients, each controls how much import is put upon a specific heuristic.
    /// A general guideline is C1 > C3 > C2 with C1 being farily large in comparison and C2 generally being very small.
    /// </summary>
    private readonly int C1 = 12; // Material value.
    private readonly int C2 = 1; // Mobility value.
    private readonly int C3 = 3; // Pawn rank value.

    /// <summary>
    /// Move buffer for AI, prevents move reduplication within the last few moves.
    /// This should be relatively small, perhaps ideally in the single digits.
    /// Much larger and the AI will tend to forfeit early.
    /// </summary>
    private readonly int BUFFER_SIZE = 3;

    private readonly int STALEMATE = 0;
    private readonly int DRAW = 0;
    private readonly int CHECK = 2;
    /// <summary>
    /// Checkmate evaluation value should be higher than any other possible evaluation value.
    /// </summary>
    private readonly int CHECKMATE = 4000;

    /// <summary>
    /// Initial values for alpha and beta.
    /// </summary>
    private readonly int UPPER_BOUND = 99999;
    private readonly int LOWER_BOUND = -99999;

    public readonly int Depth = depth;

    private readonly Random random = new();
    private readonly List<string> Buffer = [];
    private int EvaluationCount = 0;
    private int PruneCount = 0;
    private int BestMoveValue = 0;

    public override Move? PromptMove(Board board)
    {
        EvaluationCount = 0;
        PruneCount = 0;
        BestMoveValue = LOWER_BOUND;
        Board boardClone;

        int alpha = LOWER_BOUND;
        int beta = UPPER_BOUND;

        List<Move> eligibleMoves = board.GetAllMoves(this.Color, false, true);
        List<Move> bestMoves = [];

        foreach (Move eligibleMove in eligibleMoves)
        {
            // To avoid threefold repetition, we need to handle cases where a move is repeated.
            // Mostly evident in AI vs. AI games where it will go on forever as they move pieces back and forth ad infinitum.
            // This prevents reuse of previous moves the amount of which is determined by a suffer.
            // Added benefits of less negamax calls meaning faster speed.
            if (Buffer.Contains(eligibleMove.StateMove()))
            {
                continue;
            }

            boardClone = new(board);
            boardClone.MovePiece(eligibleMove);

            int negamaxValue = -Negamax(boardClone, Depth - 1, -beta, -alpha, Constants.GetColorInvert(this.Color));

            if (negamaxValue == BestMoveValue || eligibleMoves.Count < BUFFER_SIZE)
            {
                bestMoves.Add(eligibleMove);
            }
            else if (negamaxValue > BestMoveValue)
            {
                BestMoveValue = negamaxValue;
                bestMoves.Clear();
                bestMoves.Add(eligibleMove);
            }

            if (BestMoveValue > alpha)
            {
                alpha = BestMoveValue;
            }

            if (alpha >= beta)
            {
                PruneCount++;
                break;
            }
        }

        if (bestMoves.Count == 0)
        {
            // If there are no moves worth making, returning null means the AI forfeits.
            return null;
        }

        Move moveToMake = bestMoves[random.Next(bestMoves.Count)];
        Buffer.Add(moveToMake.StateMove());

        if (Buffer.Count > BUFFER_SIZE)
        {
            Buffer.RemoveAt(0);
        }

        boardClone = new(board);
        boardClone.MovePiece(moveToMake);

        if (boardClone.IsCheckmate(Constants.GetColorInvert(this.Color)))
        {
            BestMoveValue = CHECKMATE;
        }
        else if (boardClone.IsStalemate(Constants.GetColorInvert(this.Color)))
        {
            BestMoveValue = STALEMATE;
        }
        else if (boardClone.IsDraw())
        {
            BestMoveValue = DRAW;
        }
        else if (boardClone.IsCheck(Constants.GetColorInvert(this.Color)))
        {
            BestMoveValue = CHECK * EvaluateBoard(boardClone);
        }
        else
        {
            BestMoveValue = EvaluateBoard(boardClone);
        }

        Console.WriteLine($"{EvaluationCount} game state(s) evaluated; {PruneCount} pruned.");
        Console.WriteLine($"Chose move with a score of {BestMoveValue}.\n");

        return moveToMake;
    }

    private int Negamax(Board board, int depth, int alpha, int beta, Constants.Color currentColor)
    {
        EvaluationCount++;

        int offset = currentColor == this.Color ? 1 : -1;

        if (board.IsCheckmate(currentColor))
        {
            return offset * CHECKMATE;
        }
        
        if (board.IsStalemate(currentColor))
        {
            return STALEMATE;
        }

        if (board.IsDraw())
        {
            return DRAW;
        }

        if (depth == 0)
        {
            return offset * EvaluateBoard(board);
        }

        int result = LOWER_BOUND;
        List<Move> eligibleMoves = board.GetAllMoves(currentColor, false, true);

        foreach (Move eligibleMove in eligibleMoves)
        {
            Board boardClone = new(board);
            boardClone.MovePiece(eligibleMove);

            int negamaxValue = -Negamax(boardClone, depth - 1, -beta, -alpha, Constants.GetColorInvert(currentColor));

            if (boardClone.IsCheck(currentColor))
            {
                negamaxValue *= CHECK;
            }

            result = Math.Max(result, negamaxValue);
            alpha = Math.Max(alpha, result);

            if (alpha > beta)
            {
                PruneCount++;
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluates the board using the C1, C2 and C3 coefficients.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <returns>The board evaluation score, higher means a better position.</returns>
    private int EvaluateBoard(Board board)
    {
        return 
            (C1 * board.GetAllPieceMaterialValues(this.Color)) +
            (C2 * board.GetAllMobilityValues(this.Color)) +
            (C3 * board.GetAllPawnRankValues(this.Color));
    }
}