using System.Diagnostics;

namespace Chess;

/// <summary>
/// Represents any chess piece in a game of chess.
/// It defines the common properties between pieces as well as the interface that a piece should implement.
/// </summary>
/// <param name="color">The color of this piece.</param>
/// <param name="pieceType">The type of piece.</param>
abstract class Piece(Constants.Color color, Constants.PieceType pieceType)
{
    public readonly Constants.Color Color = color;
    /// <summary>
    /// Used for pawns moving two spaces forward as well as castling.
    /// </summary>
    public bool HasMoved { get; private set; } = false;
    public readonly Constants.PieceType PieceType = pieceType;

    /// <summary>
    /// Creates a clone of this piece.
    /// </summary>
    /// <returns>A clone of this piece.</returns>
    public abstract Piece Clone();

    /// <summary>
    /// Evaluates all possible moves this piece can make in a given board state from a given position.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="originRow">The current rank, or row.</param>
    /// <param name="originColumn">The current file, or column.</param>
    /// <returns>A list of all possible moves this piece can make.</returns>
    public abstract List<Move> GetMoves(Board board, int originRow, int originColumn);

    /// <summary>
    /// Evaluates whether the given coordinates are in bounds.
    /// </summary>
    /// <param name="row">The given rank, or row.</param>
    /// <param name="column">The given file, or column.</param>
    /// <returns>True, if the given coordinates are in bounds.</returns>
    protected static bool IsInBounds(int row, int column)
    {
        return row < Board.ROWS && row >= 0 && column < Board.COLUMNS && column >= 0;
    }

    /// <summary>
    /// This should be called whenever a piece has moved.
    /// </summary>
    public void SetHasMoved()
    {
        HasMoved = true;
    }
}