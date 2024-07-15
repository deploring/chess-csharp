namespace Chess;

class Program
{
    /// <summary>
    /// Prompts the user for their input.
    /// </summary>
    /// <returns>The user's input.</returns>
    public static string Prompt()
    {
        Console.Write("Input: ");
        string result = Console.ReadLine() ?? "";
        Console.WriteLine();
        return result;
    }

    /// <summary>
    /// Takes user input and instantiates a new computer player.
    /// </summary>
    /// <param name="color">The color of this computer player.</param>
    /// <returns>A new computer player.</returns>
    private static ComputerPlayer AddComputerPlayer(Constants.Color color)
    {
        while (true)
        {
            Console.WriteLine("Please enter the number of moves that the computer will think ahead.\n");
            string input = Prompt();

            if (int.TryParse(input, out int depth))
            {
                if (depth <= 0)
                {
                    Console.WriteLine("Please enter a number greater than zero.\n");
                    continue;
                }
                else if (depth > 4)
                {
                    Console.WriteLine("WARNING: Thinking more than 4 moves ahead may produce considerable wait times.");
                }

                return new(color, depth);
            }
            else
            {
                Console.WriteLine("Please enter a valid number.\n");
            }
        }
    }

    /// <summary>
    /// Takes user input and instantiates either a human or computer player.
    /// </summary>
    /// <param name="color">The color of this player.</param>
    /// <returns>A new player.</returns>
    private static Player AddPlayer(Constants.Color color)
    {
        Console.WriteLine($"Please select the player type for {color}. Type 'human' for human, and type 'computer' for computer.\n");

        while (true)
        {
            string input = Prompt().ToLower();

            switch (input)
            {
                case "human":
                    return new HumanPlayer(color);
                case "computer":
                    return AddComputerPlayer(color);
                default:
                    Console.WriteLine("Unknown player type. Please try again.\n");
                    break;
            }
        }
    }

    /// <summary>
    /// Main program loop. It will continually set up and run a game until the user inputs "quit".
    /// </summary>
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Welcome to C# Chess (Console Edition)!\n");

            Player playerWhite = AddPlayer(Constants.Color.White);
            Player playerBlack = AddPlayer(Constants.Color.Black);

            Game game = new(playerWhite, playerBlack);
            game.Play();

            Console.WriteLine("Good game! Please type 'quit' if you would like to quit, otherwise anything else to play another game.\n");
            string input = Prompt().ToLower();

            if (input == "quit")
            {
                break;
            }
        }
    }
}
