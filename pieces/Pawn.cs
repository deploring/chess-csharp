namespace Chess;

/// <summary>
/// Represents a pawn piece in a chess game. 
/// This piece can only move forward and can only capture diagonally. It may move two spaces forward on its first move.
/// </summary>
/// <param name="color">The color of this pawn.</param>
class Pawn(Constants.Color color) : Piece(color, Constants.PieceType.Pawn)
{
    /// <summary>
    /// The different types of moves a pawn can make.
    /// </summary>
    private enum MoveType
    {
        OneSpace,
        TwoSpaces,
        WestDiagonal,
        EastDiagonal
    }

    public override Pawn Clone()
    {
        Pawn result = new(Color);

        if (HasMoved) {
            result.SetHasMoved();
        }

        return result;
    }

    /// <summary>
    /// Determines all possible moves this Pawn can make at the given coordinates in a given board state.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <param name="originRow">The row where this Pawn is located.</param>
    /// <param name="originColumn">The column where this Pawn is located.</param>
    /// <returns>All possible moves this Pawn can make.</returns>
    /// <see cref="Board"/>
    public override List<Move> GetMoves(Board board, int originRow, int originColumn)
    {
        List<Move> result = [];
        int offset = Constants.GetColorOffset(Color);

        foreach (MoveType moveType in (MoveType[]) Enum.GetValues(typeof(MoveType)))
        {
            if (moveType == MoveType.TwoSpaces && HasMoved)
            {
                continue;
            }

            int destinationRow = originRow;
            int destinationColumn = originColumn;

            switch (moveType)
            {
                case MoveType.OneSpace:
                    destinationRow += 1 * offset;
                    break;
                case MoveType.TwoSpaces:
                    destinationRow += 2 * offset;
                    break;
                case MoveType.WestDiagonal:
                    destinationRow += 1 * offset;
                    destinationColumn -= 1;
                    break;
                case MoveType.EastDiagonal:
                    destinationRow += 1 * offset;
                    destinationColumn += 1;
                    break;
            }

            if (IsInBounds(destinationRow, destinationColumn))
            {
                Piece? destinationPiece = board.Tiles[destinationRow, destinationColumn].Occupant;

                if ((moveType == MoveType.OneSpace || moveType == MoveType.TwoSpaces) && destinationPiece != null)
                {
                    // Pawn cannot capture moving forward.
                    continue;
                }

                if (moveType == MoveType.TwoSpaces && board.Tiles[destinationRow - (1 * offset), destinationColumn].Occupant != null)
                {
                    // Pawn cannot jump over a piece.
                    continue;
                }

                if ((moveType == MoveType.EastDiagonal || moveType == MoveType.WestDiagonal) && destinationPiece != null)
                {
                    if (destinationPiece.Color != this.Color)
                    {
                        // Enemy piece positioned diagonally to the front of this pawn can be captured.
                        result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                    }
                }
                else if (moveType != MoveType.EastDiagonal && moveType != MoveType.WestDiagonal)
                {
                    // Space is not occupied, pawn can move forward.
                    result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                }
            }
        }

        return result;
    }
}