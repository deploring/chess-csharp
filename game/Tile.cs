using System.Diagnostics;

namespace Chess;

/// <summary>
/// Represents a tile on a chess board, which a piece can occupy.
/// </summary>
/// <see cref="Board"/>
class Tile 
{
    public Piece? Occupant {get; private set;}

    public Tile()
    {
        Occupant = null;
    }

    public void SetOccupant(Piece occupant)
    {
        Occupant = occupant;
    }

    public void Clear()
    {
        Debug.Assert(Occupant != null, "Cannot clear a Tile with no Occupant.");
        Occupant = null;
    }
}