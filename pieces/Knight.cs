namespace Chess;

/// <summary>
/// Represents a knight piece in a chess game. 
/// This piece can move in an L shape in all directions and can jump over other pieces.
/// </summary>
/// <param name="color">The color of this knight.</param>
class Knight(Constants.Color color) : Piece(color, Constants.PieceType.Knight)
{
    /// <summary>
    /// The L-shaped directions a knight can move in.
    /// </summary>
    private enum Direction
    {
        NorthNorthEast,
        NorthNorthWest,
        EastEastNorth,
        EastEastSouth,
        SouthSouthEast,
        SouthSouthWest,
        WestWestNorth,
        WestWestSouth
    }

    public override Knight Clone()
    {
        Knight result = new(Color);

        if (HasMoved) {
            result.SetHasMoved();
        }

        return result;
    }

    /// <summary>
    /// Determines all possible moves this Knight can make at the given coordinates in a given board state.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <param name="originRow">The row where this Knight is located.</param>
    /// <param name="originColumn">The column where this Knight is located.</param>
    /// <returns>All possible moves this Knight can make.</returns>
    /// <see cref="Board"/>
    public override List<Move> GetMoves(Board board, int originRow, int originColumn)
    {
        List<Move> result = [];

        foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
        {
            int destinationRow = originRow;
            int destinationColumn = originColumn;

            switch (direction)
            {
                case Direction.NorthNorthEast:
                    destinationRow += 2;
                    destinationColumn += 1;
                    break;
                case Direction.NorthNorthWest:
                    destinationRow += 2;
                    destinationColumn -= 1;
                    break;
                case Direction.EastEastNorth:
                    destinationRow += 1;
                    destinationColumn += 2;
                    break;
                case Direction.EastEastSouth:
                    destinationRow -= 1;
                    destinationColumn += 2;
                    break;
                case Direction.SouthSouthEast:
                    destinationRow -= 2;
                    destinationColumn += 1;
                    break;
                case Direction.SouthSouthWest:
                    destinationRow -= 2;
                    destinationColumn -= 1;
                    break;
                case Direction.WestWestNorth:
                    destinationRow += 1;
                    destinationColumn -= 2;
                    break;
                case Direction.WestWestSouth:
                    destinationRow -= 1;
                    destinationColumn -= 2;
                    break;
            }

            if (IsInBounds(destinationRow, destinationColumn))
            {
                Piece? destinationPiece = board.Tiles[destinationRow, destinationColumn].Occupant;

                if (destinationPiece != null)
                {
                    if (destinationPiece.Color != this.Color)
                    {
                        // Capturing the opponent's piece is a valid move.
                        result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                    } 
                    
                    // Otherwise, cannot capture a friendly piece.
                }
                else
                {
                    // Tile is not occupied, this is a valid move.
                    result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                }
            }
        }

        return result;
    }
}