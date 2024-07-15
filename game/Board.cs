using System.Diagnostics;

namespace Chess;

/// <summary>
/// Represents the state of a chess board, which contains an 8x8 grid of tiles where pieces can reside.
/// </summary>
class Board
{
    private static readonly string FILE_INDICATOR = "    a   b   c   d   e   f   g   h\n";
    private static readonly string BOARD_SEPARATOR = "  +---+---+---+---+---+---+---+---+\n";
    public static readonly int ROWS = 8;
    public static readonly int COLUMNS = 8;

    public readonly Tile[,] Tiles = new Tile[ROWS, COLUMNS];

    /// <summary>
    /// Instantiates a fresh Board with all pieces in their starting positions.
    /// </summary>
    public Board()
    {
        InitialiseTiles();
        InitialisePieces();
    }
    
    /// <summary>
    /// Instantiates a deep clone of the provided Board.
    /// </summary>
    /// <param name="board">The Board to clone.</param>
    public Board(Board board)
    {
        InitialiseTiles();

        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = board.Tiles[row, column].Occupant;

                if (piece == null)
                {
                    continue;
                }

                Tiles[row, column].SetOccupant(piece.Clone());
            }
        }
    }

    /// <summary>
    /// Initialises the Tiles array with empty Tile objects.
    /// </summary>
    private void InitialiseTiles()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Tiles[row, column] = new();
            }
        }
    }

    /// <summary>
    /// Populates an empty Board with the standard chess piece layout.
    /// </summary>
    private void InitialisePieces()
    {
        Tiles[0, 0].SetOccupant(new Rook(Constants.Color.White));
        Tiles[0, 1].SetOccupant(new Knight(Constants.Color.White));
        Tiles[0, 2].SetOccupant(new Bishop(Constants.Color.White));
        Tiles[0, 3].SetOccupant(new Queen(Constants.Color.White));
        Tiles[0, 4].SetOccupant(new King(Constants.Color.White));
        Tiles[0, 5].SetOccupant(new Bishop(Constants.Color.White));
        Tiles[0, 6].SetOccupant(new Knight(Constants.Color.White));
        Tiles[0, 7].SetOccupant(new Rook(Constants.Color.White));

        for (int column = 0; column < COLUMNS; column++)
        {
            Tiles[1, column].SetOccupant(new Pawn(Constants.Color.White));
        }

        Tiles[7, 0].SetOccupant(new Rook(Constants.Color.Black));
        Tiles[7, 1].SetOccupant(new Knight(Constants.Color.Black));
        Tiles[7, 2].SetOccupant(new Bishop(Constants.Color.Black));
        Tiles[7, 3].SetOccupant(new Queen(Constants.Color.Black));
        Tiles[7, 4].SetOccupant(new King(Constants.Color.Black));
        Tiles[7, 5].SetOccupant(new Bishop(Constants.Color.Black));
        Tiles[7, 6].SetOccupant(new Knight(Constants.Color.Black));
        Tiles[7, 7].SetOccupant(new Rook(Constants.Color.Black));

        for (int column = 0; column < COLUMNS; column++)
        {
            Tiles[6, column].SetOccupant(new Pawn(Constants.Color.Black));
        }
    }

    /// <summary>
    /// Builds a user-friendly string describing the state of the Board.
    /// </summary>
    /// <param name="previousMove">Supplying this will show the previous position of the last moved piece.</param>
    /// <param name="moves">All possible moves the current player can make. Supplying this along with the origin coordinates will show where a selected piece can move.</param>
    /// <param name="originRow">Supplying this along with `moves` will show where a selected piece can move.</param>
    /// <param name="originColumn">Supplying this along with `moves` will show where a selected piece can move.</param>
    /// <returns>The state of the Board, as a string which can be printed to the console.</returns>
    public string StateBoard(Move? previousMove = null, List<Move>? moves = null, int originRow = -1, int originColumn = -1)
    {
        string result = FILE_INDICATOR + BOARD_SEPARATOR;

        for (int row = ROWS - 1; row >= 0; row--)
        {
            result += $"{row + 1} |";

            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = Tiles[row, column].Occupant;
                char pieceDisplayValue = ' ';

                if (piece != null)
                {
                    pieceDisplayValue = Constants.GetPieceDisplayValue(piece.Color, piece.PieceType);
                }

                // Shows a '~' char on the previous position of this last moved piece.
                if (previousMove != null && previousMove.OriginRow == row && previousMove.OriginColumn == column)
                {
                    Debug.Assert(piece == null, "This piece should have moved!");
                    pieceDisplayValue = '~';
                }

                char moveDisplayValue = ' ';

                if (originRow == row && originColumn == column)
                {
                    // Wraps this piece in '*' chars to indicate it is the piece the user wishes to move.
                    moveDisplayValue = '*';
                }
                else if (moves != null)
                {
                    foreach (Move move in moves)
                    {
                        if (move.OriginRow != originRow || move.OriginColumn != originColumn)
                        {
                            // This move does not pertain to the selected piece being moved.
                            continue;
                        }

                        if (move.DestinationRow == row && move.DestinationColumn == column)
                        {
                            // Wraps this tile in '!' chars to indicate it is a tile where the selected piece can move.
                            moveDisplayValue = '!';
                        }
                    }
                }

                // Wraps this tile in '~' chars to indicate the new position of the last moved piece.
                if (previousMove != null && previousMove.DestinationRow == row && previousMove.DestinationColumn == column)
                {
                    Debug.Assert(piece != null, "This piece should have moved here!");
                    moveDisplayValue = '~';
                }

                result += $"{moveDisplayValue}{pieceDisplayValue}{moveDisplayValue}|";
            }

            result += $" {row + 1}\n" + BOARD_SEPARATOR;
        }

        result += FILE_INDICATOR;

        return result;
    }

    /// <summary>
    /// Takes a given move and applies it to the Board state.
    /// </summary>
    /// <param name="move">The given move to make.</param>
    public void MovePiece(Move move)
    {
        Tile originTile = Tiles[move.OriginRow, move.OriginColumn];
        Debug.Assert(originTile.Occupant != null, "Origin tile does not have an occupying piece!");

        Constants.Color color = originTile.Occupant.Color;
        Tile destinationTile = Tiles[move.DestinationRow, move.DestinationColumn];

        if (destinationTile.Occupant != null)
        {
            Debug.Assert(destinationTile.Occupant.Color != originTile.Occupant.Color, "Cannot capture your own piece!");
            Debug.Assert(destinationTile.Occupant is not King, "Cannot capture king!");
            destinationTile.Clear();
        }

        switch (originTile.Occupant.PieceType)
        {
            case Constants.PieceType.Bishop:
            case Constants.PieceType.Knight:
            case Constants.PieceType.Queen:
            case Constants.PieceType.Rook:
                destinationTile.SetOccupant(originTile.Occupant.Clone());
                break;
            case Constants.PieceType.Pawn:
                // Pawn promotion.
                if (
                    (move.DestinationRow == ROWS - 1 && color == Constants.Color.White) ||
                    (move.DestinationRow == 0 && color == Constants.Color.Black)
                )
                {
                    destinationTile.SetOccupant(new Queen(color));
                }
                else
                {
                    destinationTile.SetOccupant(originTile.Occupant.Clone());
                }
                break;
            case Constants.PieceType.King:
                // Kingside castling check.
                if (move.OriginColumn == 4 && move.DestinationColumn == 6)
                {
                    Debug.Assert(!originTile.Occupant.HasMoved, "Cannot castle if king has moved!");

                    Tiles[move.OriginRow, 7].Clear();
                    Tiles[move.OriginRow, 5].SetOccupant(new Rook(color));
                    Tiles[move.OriginRow, 5].Occupant.SetHasMoved();
                }
                // Queenside castling check.
                else if (move.OriginColumn == 4 && move.DestinationColumn == 2)
                {
                    Debug.Assert(!originTile.Occupant.HasMoved, "Cannot castle if king has moved!");

                    Tiles[move.OriginRow, 0].Clear();
                    Tiles[move.OriginRow, 3].SetOccupant(new Rook(color));
                    Tiles[move.OriginRow, 3].Occupant.SetHasMoved();
                }

                destinationTile.SetOccupant(originTile.Occupant.Clone());
                break;
        }

        destinationTile.Occupant.SetHasMoved();
        originTile.Clear();
    }

    /// <summary>
    /// Calculates all possible moves a given color can make.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <param name="ignoreKing">True, if the king's moves should be excluded.</param>
    /// <param name="removeCheckedMoves">True, if moves that would put the king in check should be removed.</param>
    /// <returns>A list of all moves the given color can make.</returns>
    public List<Move> GetAllMoves(Constants.Color color, bool ignoreKing, bool removeCheckedMoves)
    {
        List<Move> result = [];

        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = Tiles[row, column].Occupant;

                if (piece == null || piece.Color != color)
                {
                    // Only existing friendly pieces are relevant.
                    continue;
                }

                if (ignoreKing && piece is King)
                {
                    // This is to prevent infinite recursion when calculating check.
                    // As a result, a king's moves must always be checked such that they do not place two kings adjacent to each other.
                    // This is done in the King class.
                    continue;
                }

                result.AddRange(piece.GetMoves(this, row, column));
            }
        }

        if (removeCheckedMoves)
        {
            RemoveCheckedMoves(this, color, result);
        }

        return result;
    }

    /// <summary>
    /// Evaluates if the given color is in check.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>True, if the given color is in check.</returns>
    public bool IsCheck(Constants.Color color)
    {
        List<Move> moves = GetAllMoves(Constants.GetColorInvert(color), true, false);

        foreach (Move move in moves)
        {
            Piece? destinationPiece = Tiles[move.DestinationRow, move.DestinationColumn].Occupant;

            if (destinationPiece != null && destinationPiece is King)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Takes a list of possible moves and removes any moves that would put the king in check.
    /// </summary>
    /// <param name="board">The current state of the Board.</param>
    /// <param name="color">The color of the current player.</param>
    /// <param name="moves">All possible moves the current player can make.</param>
    public static void RemoveCheckedMoves(Board board, Constants.Color color, List<Move> moves)
    {
        List<Move> checkedMoves = [];

        foreach (Move move in moves)
        {
            Board boardClone = new(board);
            boardClone.MovePiece(move);

            if (boardClone.IsCheck(color))
            {
                checkedMoves.Add(move);
            }
        }

        foreach (Move moveToRemove in checkedMoves)
        {
            moves.Remove(moveToRemove);
        }
    }

    /// <summary>
    /// Evaluates if the given color is in checkmate.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>True, if the given color is in checkmate.</returns>
    public bool IsCheckmate(Constants.Color color)
    {
        if (!IsCheck(color))
        {
            return false;
        }

        return GetAllMoves(color, false, true).Count == 0;
    }

    /// <summary>
    /// Evaluates if the given color is in stalemate.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>True, if the given color is in stalemate.</returns>
    public bool IsStalemate(Constants.Color color)
    {
        if (IsCheck(color))
        {
            return false;
        }

        return GetAllMoves(color, false, true).Count == 0;
    }

    /// <summary>
    /// Evaluates if the game has ended in a draw. This happens when there are only two kings left.
    /// </summary>
    /// <returns>True, if the game has ended in a draw.</returns>
    public bool IsDraw()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = Tiles[row, column].Occupant;

                if (piece != null && piece is not King)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Evaluates the sum of the piece material value for a given color.
    /// This is essentially the sum of piece material value for the given color minus the sum of piece material value for the opponent.
    /// This is used by the AI as part of its decision making.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>Piece material value for the given color.</returns>
    public int GetAllPieceMaterialValues(Constants.Color color)
    {
        int result = 0;

        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = Tiles[row, column].Occupant;

                if (piece != null)
                {
                    int pieceMaterialValue = Constants.GetPieceMaterialValue(piece.PieceType);

                    if (piece.Color == color)
                    {
                        result += pieceMaterialValue;
                    }
                    else
                    {
                        result -= pieceMaterialValue;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluates the sum of mobility value for a given color.
    /// This is essentially the number of moves the given color can make minus the number of moves the opponent can make.
    /// This is used by the AI as part of its decision making.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>Mobility value for the given color.</returns>
    public int GetAllMobilityValues(Constants.Color color)
    {
        int result = 0;

        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = Tiles[row, column].Occupant;

                if (piece == null)
                {
                    continue;
                }

                int moveCount = piece.GetMoves(this, row, column).Count;

                if (piece.Color == color)
                {
                    result += moveCount;
                }
                else {
                    result -= moveCount;
                }
            }
        }
        
        return result;
    }

    /// <summary>
    /// Evaluates the sum of pawn rank values for a given color.
    /// This is essentially how far the given color's pawns are pushed forward compared to the opponent.
    /// This is used by the AI as part of its decision making.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public int GetAllPawnRankValues(Constants.Color color)
    {
        int result = 0;

        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                Piece? piece = Tiles[row, column].Occupant;

                if (piece == null || piece is not Pawn)
                {
                    continue;
                }

                if (piece.Color == color)
                {
                    if (color == Constants.Color.White)
                    {
                        result += row - 1;
                    }
                    else
                    {
                        Debug.Assert(color == Constants.Color.Black, "Unsupported color.");
                        result += 6 - row;
                    }
                }
                else
                {
                    if (color == Constants.Color.White)
                    {
                        result -= 6 - row;
                    }
                    else
                    {
                        Debug.Assert(color == Constants.Color.Black, "Unsupported color.");
                        result -= row - 1;
                    }
                }
            }
        }

        return result;
    }
}