using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Chess;

/// <summary>
/// Represents a king piece in a chess game. This piece can move in all directions over 1 tile.
/// </summary>
/// <param name="color">The color of this king.</param>
class King(Constants.Color color) : Piece(color, Constants.PieceType.King)
{
    /// <summary>
    /// The directions a king can move in.
    /// </summary>
    private enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public override King Clone()
    {
        King result = new(Color);

        if (HasMoved) {
            result.SetHasMoved();
        }

        return result;
    }

    /// <summary>
    /// Takes destination coordinates and updates them based on the provided Direction.
    /// </summary>
    /// <param name="direction">The direction in which the King is moving.</param>
    /// <param name="destinationRow">The current destination row to be updated.</param>
    /// <param name="destinationColumn">The current destination column to be updated.</param>
    private static void DoMove(Direction direction, ref int destinationRow, ref int destinationColumn)
    {
        switch (direction)
        {
            case Direction.North:
                destinationRow += 1;
                break;
            case Direction.NorthEast:
                destinationRow += 1;
                destinationColumn += 1;
                break;
            case Direction.East:
                destinationColumn += 1;
                break;
            case Direction.SouthEast:
                destinationRow -= 1;
                destinationColumn += 1;
                break;
            case Direction.South:
                destinationRow -= 1;
                break;
            case Direction.SouthWest:
                destinationRow -= 1;
                destinationColumn -= 1;
                break;
            case Direction.West:
                destinationColumn -= 1;
                break;
            case Direction.NorthWest:
                destinationRow += 1;
                destinationColumn -= 1;
                break;
        }
    }

    /// <summary>
    /// Evaluates if castling is possible for this King and adds the castling moves if they are possible.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <param name="originRow">The row where this King is located.</param>
    /// <param name="originColumn">The column where this King is located.</param>
    /// <param name="moves">The moves this King can make, used by GetMoves.</param>
    /// <see cref="GetMoves"/>
    private void AddCastleMoves(Board board, int originRow, int originColumn, List<Move> moves)
    {
        if (HasMoved)
        {
            // Castling cannot be done if the king has already moved.
            return;
        }

        Debug.Assert(originColumn == 4, "King should not have moved!");

        if (board.IsCheck(this.Color))
        {
            // Castling cannot be done if the king is in check.
            return;
        }

        bool canCastle = true;

        // Castle kingside check.
        for (int column = originColumn + 1; column < Board.COLUMNS; column++)
        {
            Piece? piece = board.Tiles[originRow, column].Occupant;

            if (column < Board.COLUMNS - 1)
            {
                if (piece != null)
                {
                    // Castling cannot be done if the space between the king and the rook is not clear.
                    canCastle = false;
                    break;
                }
            }

            if (column == originColumn + 1)
            {
                Board boardClone = new(board);
                boardClone.MovePiece(new(originRow, originColumn, originRow, column));

                if (boardClone.IsCheck(this.Color))
                {
                    canCastle = false;
                    break;
                }
            }

            if (column == Board.COLUMNS - 1)
            {
                if (piece == null || piece.HasMoved)
                {
                    // Castling cannot be done if the rook has moved.
                    canCastle = false;
                }
            }
        }

        if (canCastle)
        {
            moves.Add(new(originRow, originColumn, originRow, originColumn + 2));
        }

        canCastle = true;

        // Castle queenside check.
        for (int column = originColumn - 1; column >= 0; column--)
        {
            Piece? piece = board.Tiles[originRow, column].Occupant;

            if (column > 0)
            {
                if (piece != null)
                {
                    // Castling cannot be done if the space between the king and the rook is not clear.
                    canCastle = false;
                    break;
                }
            }

            if (column == originColumn - 1)
            {
                Board boardClone = new(board);
                boardClone.MovePiece(new(originRow, originColumn, originRow, column));

                if (boardClone.IsCheck(this.Color))
                {
                    canCastle = false;
                    break;
                }
            }

            if (column == 0)
            {
                if (piece == null || piece.HasMoved)
                {
                    // Castling cannot be done if the rook has moved.
                    canCastle = false;
                }
            }
        }

        if (canCastle)
        {
            moves.Add(new(originRow, originColumn, originRow, originColumn - 2));
        }
    }

    /// <summary>
    /// Removes moves which would put a king adjacent to another king as these are not detected in GetMoves.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <param name="moves">The moves this King can make, which may include moves that puts it adjacent to another King.</param>
    /// <see cref="Board"/>
    private void RemoveAdjacentKingMoves(Board board, List<Move> moves)
    {
        List<Move> adjacentKingMoves = [];

        foreach (Move move in moves)
        {
            foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
            {
                int destinationRow = move.DestinationRow;
                int destinationColumn = move.DestinationColumn;

                DoMove(direction, ref destinationRow, ref destinationColumn);

                if (IsInBounds(destinationRow, destinationColumn))
                {
                    Piece? destinationPiece = board.Tiles[destinationRow, destinationColumn].Occupant;

                    if (destinationPiece != null && destinationPiece.Color != this.Color && destinationPiece is King)
                    {
                        // This move makes the king adjacent to another king, so it should be removed.
                        adjacentKingMoves.Add(move);
                    }
                }
            }
        }

        foreach (Move moveToRemove in adjacentKingMoves)
        {
            moves.Remove(moveToRemove);
        }
    }

    /// <summary>
    /// Determines all possible moves this King can make at the given coordinates in a given board state.
    /// </summary>
    /// <param name="board">The current Board state.</param>
    /// <param name="originRow">The row where this King is located.</param>
    /// <param name="originColumn">The column where this King is located.</param>
    /// <returns>All possible moves this King can make.</returns>
    /// <see cref="Board"/>
    public override List<Move> GetMoves(Board board, int originRow, int originColumn)
    {
        List<Move> result = [];

        foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
        {
            int destinationRow = originRow;
            int destinationColumn = originColumn;

            DoMove(direction, ref destinationRow, ref destinationColumn);

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
                }
                else
                {
                    // Tile is not occupied, this is a valid move.
                    result.Add(new(originRow, originColumn, destinationRow, destinationColumn));
                }
            }
        }

        AddCastleMoves(board, originRow, originColumn, result);

        Board.RemoveCheckedMoves(board, this.Color, result);
        RemoveAdjacentKingMoves(board, result);

        return result;
    }
}