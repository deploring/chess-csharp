namespace Chess;

/// <summary>
/// Represents any type of player in this program.
/// </summary>
/// <param name="color"></param>
abstract class Player(Constants.Color color)
{
    public readonly Constants.Color Color = color;

    public abstract Move? PromptMove(Board board);
}