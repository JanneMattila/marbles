using System.Security.Cryptography;

namespace MarblesSimple;

public class Game
{
    public int YourMarbles { get; private set; }
    public int OpponentMarbles { get; private set; }

    public GameState State { get; private set; }

    private Random _random = new Random();

    public void StartGame()
    {
        YourMarbles = 10;
        OpponentMarbles = 10;
        State = GameState.ChooseNumberOfMarbles;
    }

    public void SelectMarbles(int count)
    {
        if (State != GameState.ChooseNumberOfMarbles) throw new Exception();

        var oddOrEven = RandomNumberGenerator.GetInt32(1);
        if (oddOrEven == 0 /* odd */ && count % 2 == 0)
        {
            MessageBox.Show($"Your opponent guessed odd and got it right. You lost {count} marbles.", "Opponent won this round", MessageBoxButtons.OK);
            YourMarbles -= count;
            OpponentMarbles += count;
        }
        else if (oddOrEven == 1 /* event */ && count % 2 != 0)
        {
            MessageBox.Show($"Your opponent guessed even and got it right. You lost {count} marbles.", "Opponent won this round", MessageBoxButtons.OK);
            YourMarbles -= count;
            OpponentMarbles += count;
        }
        else if (oddOrEven == 0 /* odd */)
        {
            MessageBox.Show($"Your opponent guessed odd and got it wrong.", "Opponent won this round", MessageBoxButtons.OK);
        }
        else if (oddOrEven == 1 /* event */)
        {
            MessageBox.Show($"Your opponent guessed even and got it wrong.", "Opponent won this round", MessageBoxButtons.OK);
        }

        if (YourMarbles == 20)
        {
            MessageBox.Show($"You won the game.", "Victory", MessageBoxButtons.OK);
            State = GameState.GameEnded;
        }
        else if (YourMarbles == 0)
        {
            MessageBox.Show($"You lost the game.", "Loss", MessageBoxButtons.OK);
            State = GameState.GameEnded;
        }
        else
        {
            State = GameState.ChooseOddOrEven;
        }
    }

    public void SelectOddOrEvent(bool isOdd)
    {
        if (State != GameState.ChooseOddOrEven) throw new Exception();

        var count = RandomNumberGenerator.GetInt32(OpponentMarbles);
        var oddOrEven = RandomNumberGenerator.GetInt32(1);
        if (oddOrEven == 0 /* odd */ && isOdd)
        {
            MessageBox.Show($"Your guessed odd and got it right. You got {count} marbles.", "You won this round", MessageBoxButtons.OK);
            YourMarbles += count;
            OpponentMarbles -= count;
        }
        else if (oddOrEven == 1 /* event */ && !isOdd)
        {
            MessageBox.Show($"Your guessed even and got it right. You got {count} marbles.", "You won this round", MessageBoxButtons.OK);
            YourMarbles += count;
            OpponentMarbles -= count;
        }
        else if (isOdd)
        {
            MessageBox.Show($"Your guessed odd and got it wrong since opponent placed {count} marbles.", "You lost this round", MessageBoxButtons.OK);
        }
        else if (!isOdd)
        {
            MessageBox.Show($"Your guessed even and got it wrong since opponent placed {count} marbles.", "You lost this round", MessageBoxButtons.OK);
        }

        if (YourMarbles == 20)
        {
            MessageBox.Show($"You won the game.", "Victory", MessageBoxButtons.OK);
            State = GameState.GameEnded;
        }
        else if (YourMarbles == 0)
        {
            MessageBox.Show($"You lost the game.", "Loss", MessageBoxButtons.OK);
            State = GameState.GameEnded;
        }
        else
        {
            State = GameState.ChooseNumberOfMarbles;
        }
    }
}
