namespace Chess;

/// <summary>
/// Represents a move that can be made during a game of chess.
/// It contains origin coordinates and destination coordinates.
/// </summary>
/// <param name="originRow">The origin rank, or row.</param>
/// <param name="originColumn">The origin file, or column.</param>
/// <param name="destinationRow">The destination rank, or row.</param>
/// <param name="destinationColumn">The destination file, or column.</param>
class Move(int originRow, int originColumn, int destinationRow, int destinationColumn)
{
    public readonly int OriginRow = originRow;
    public readonly int OriginColumn = originColumn;
    public readonly int DestinationRow = destinationRow;
    public readonly int DestinationColumn = destinationColumn;

    /// <summary>
    /// Builds a user-friendly description of the move.
    /// </summary>
    /// <returns>A description of the move.</returns>
    public string StateMove()
    {
        return $"{Constants.GetStringFromChessFile(OriginColumn)}{OriginRow + 1} -> {Constants.GetStringFromChessFile(DestinationColumn)}{DestinationRow + 1}";
    }

    private bool IsOriginSameAs(Move move)
    {
        return move.OriginRow == OriginRow && move.OriginColumn == OriginColumn;
    }

    private bool IsDestinationSameAs(Move move)
    {
        return move.DestinationRow == DestinationRow && move.DestinationColumn == DestinationColumn;
    }

    public bool IsSameAs(Move move)
    {
        return IsOriginSameAs(move) && IsDestinationSameAs(move);
    }
}