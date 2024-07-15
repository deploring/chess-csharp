using System.Diagnostics;

namespace Chess;

static class Constants {
    /// <summary>
    /// The two colors in a chess game.
    /// </summary>
    public enum Color
    {
        White,
        Black
    }

    /// <summary>
    /// The various different types of pieces in a chess game.
    /// </summary>
    public enum PieceType
    {
        Bishop,
        King,
        Knight,
        Pawn,
        Queen,
        Rook
    }

    /// <summary>
    /// Takes a given color and returns the opponent color.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>The inverted color.</returns>
    public static Color GetColorInvert(Color color)
    {
        return color switch
        {
            Color.White => Color.Black,
            Color.Black => Color.White,
            _ => throw new ArgumentException("Unsupported color."),
        };
    }

    /// <summary>
    /// Takes a given color and returns a movement offset.
    /// For white this is 1 as they move forward on the board.
    /// For black this is -1 as they move backward on the board.
    /// </summary>
    /// <param name="color">The given color.</param>
    /// <returns>The offset for the given color.</returns>
    public static int GetColorOffset(Color color)
    {
        return color switch
        {
            Color.White => 1,
            Color.Black => -1,
            _ => throw new ArgumentException("Unsupported color."),
        };
    }

    public static string GetColorNameLowerCase(Color color)
    {
        return color switch
        {
            Color.White => "white",
            Color.Black => "black",
            _ => throw new ArgumentException("Unsupported color."),
        };
    }

    /// <summary>
    /// Takes a given piece type and returns its material value.
    /// More advanced pieces have a higher material value.
    /// This is used by the AI to control which pieces it should prioritise protecting.
    /// </summary>
    /// <param name="pieceType">The given piece type.</param>
    /// <returns>The material value of the piece.</returns>
    public static int GetPieceMaterialValue(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Bishop => 3,
            PieceType.King => 100,
            PieceType.Knight => 3,
            PieceType.Pawn => 1,
            PieceType.Queen => 9,
            PieceType.Rook => 5,
            _ => throw new ArgumentException("Unsupported piece type."),
        };
    }

    public static char GetPieceDisplayValue(Color color, PieceType pieceType)
    {
        if (color == Color.White)
        {
            return pieceType switch
            {
                PieceType.Bishop => 'B',
                PieceType.King => 'K',
                PieceType.Knight => 'N',
                PieceType.Pawn => 'P',
                PieceType.Queen => 'Q',
                PieceType.Rook => 'R',
                _ => throw new ArgumentException("Unsupported piece type."),
            };
        }
        else
        {
            Debug.Assert(color == Color.Black, "Unsupported color.");

            return pieceType switch
            {
                PieceType.Bishop => 'b',
                PieceType.King => 'k',
                PieceType.Knight => 'n',
                PieceType.Pawn => 'p',
                PieceType.Queen => 'q',
                PieceType.Rook => 'r',
                _ => throw new ArgumentException("Unsupported piece type."),
            };
        }
    }

    public static int GetChessFileFromChar(char input)
    {
        return input switch
        {
            'a' => 0,
            'b' => 1,
            'c' => 2,
            'd' => 3,
            'e' => 4,
            'f' => 5,
            'g' => 6,
            'h' => 7,
            _ => throw new ArgumentException($"Unknown chess file \"{input}\"."),
        };
    }

    public static string GetStringFromChessFile(int chessFile)
    {
        return chessFile switch
        {
            0 => "a",
            1 => "b",
            2 => "c",
            3 => "d",
            4 => "e",
            5 => "f",
            6 => "g",
            7 => "h",
            _ => throw new ArgumentException($"Unknown chess file \"{chessFile}\"."),
        };
    }
}