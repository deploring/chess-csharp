namespace Chess;

/// <summary>
/// Represents a human player in this program.
/// It will take user inputs to 
/// </summary>
/// <param name="color"></param>
class HumanPlayer(Constants.Color color) : Player(color)
{
    public override Move? PromptMove(Board board)
    {
        List<Move> eligibleMoves = board.GetAllMoves(this.Color, false, true);

        while (true)
        {
            Console.WriteLine("Enter a move in the form of 'a1b2' (from a1 to b2).");
            Console.WriteLine("Or 'a1' to see moves (moves piece on a1 could make).");
            Console.WriteLine("Or 'resign' to quit game.");
            
            string input = Program.Prompt().ToLower();

            if (input.Equals("resign"))
            {
                return null;
            }

            if (input.Length != 4 && input.Length != 2)
            {
                Console.WriteLine("Your move input should be 2 or 4 characters long.\n");
                continue;
            }

            int originColumn;

            try 
            {
                originColumn = Constants.GetChessFileFromChar(input[0]);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Unknown origin chess file '{input[0]}'.");
                continue;
            }

            int originRow;

            try 
            {
                originRow = int.Parse(input[1].ToString()) - 1;

                if (originRow < 0 || originRow > Board.COLUMNS - 1)
                {
                    Console.WriteLine($"Unknown origin chess rank '{input[1]}'.\n");
                    continue;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unknown origin chess rank '{input[1]}'.\n");
                continue;
            }

            int destinationColumn = -1;
            int destinationRow = -1;

            if (input.Length == 4)
            {
                try 
                {
                    destinationColumn = Constants.GetChessFileFromChar(input[2]);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Unknown destination chess file '{input[2]}'.\n");
                    continue;
                }

                try 
                {
                    destinationRow = int.Parse(input[3].ToString()) - 1;

                    if (destinationRow < 0 || destinationRow > Board.COLUMNS - 1)
                    {
                        Console.WriteLine($"Unknown destination chess rank '{input[3]}'.\n");
                        continue;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unknown destination chess rank '{input[3]}'.\n");
                    continue;
                }
            }

            Piece? originPiece = board.Tiles[originRow, originColumn].Occupant;

            if (originPiece == null)
            {
                Console.WriteLine($"There is no piece at {input[0]}{input[1]}.\n");
                continue;
            }

            if (originPiece.Color != this.Color)
            {
                Console.WriteLine("This is the opponent's piece!\n");
                continue;
            }

            if (input.Length == 2)
            {
                Console.WriteLine(board.StateBoard(null, eligibleMoves, originRow, originColumn));
                continue;
            }

            Move move = new(originRow, originColumn, destinationRow, destinationColumn);

            bool validMove = false;

            foreach (Move eligibleMove in eligibleMoves)
            {
                if (eligibleMove.IsSameAs(move))
                {
                    validMove = true;
                    break;
                }
            }

            if (!validMove)
            {
                Console.WriteLine("This is an illegal move.\n");
                continue;
            }

            return move;
        }
    }
}