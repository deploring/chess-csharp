namespace Chess;

/// <summary>
/// Represents a rook piece in a chess game. 
/// This piece can move straight in all four directions over an unlimited amount of tiles.
/// </summary>
/// <param name="color">The color of this rook.</param>
class Rook(Constants.Color color) : Piece(color, Constants.PieceType.Rook)
{
    /// <summary>
    /// The straight directions a rook can move in.
    /// </summary>
    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    public override Rook Clone()
    {
        Rook result = new(Color);

        if (HasMoved) {
            result.SetHasMoved();
        }

        return result;
    }

    /// <summary>
    /// Determines all possible moves this Rook can make at the given coordinates in a given board state.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <param name="originRow">The row where this Rook is located.</param>
    /// <param name="originColumn">The column where this Rook is located.</param>
    /// <returns>All possible moves this Rook can make.</returns>
    /// <see cref="Board"/>
    public override List<Move> GetMoves(Board board, int originRow, int originColumn)
    {
        List<Move> result = [];

        foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
        {
            for (int distance = 1; distance <= 7; distance++)
            {
                int destinationRow = originRow;
                int destinationColumn = originColumn;

                switch (direction)
                {
                    case Direction.North:
                        destinationRow += distance;
                        break;
                    case Direction.East:
                        destinationColumn += distance;
                        break;
                    case Direction.South:
                        destinationRow -= distance;
                        break;
                    case Direction.West:
                        destinationColumn -= distance;
                        break;
                }

                if (IsInBounds(destinationRow, destinationColumn))
                {
                    Piece? destinationPiece = board.Tiles[destinationRow, destinationColumn].Occupant;

                    if (destinationPiece != null)
                    {
                        if (destinationPiece.Color != this.Color)
                        {
                            // Capturing the enemy piece is a valid move.
                            result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                        } 

                        // Otherwise, cannot capture a friendly piece.

                        // Cannot move over pieces.
                        break;
                    }
                    else
                    {
                        // Tile is not occupied, this is a valid move.
                        result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                    }
                }
                else 
                {
                    // Cannot move out of bounds.
                    break;
                }
            } 
        }

        return result;
    }
}