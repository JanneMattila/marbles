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

        var evenOrOdd = RandomNumberGenerator.GetInt32(1);
        if (evenOrOdd == 0 /* even */ && count % 2 != 0)
        {
            MessageBox.Show($"Your opponent guessed even and got it right. You lost {count} marbles.", "Opponent won this round", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            YourMarbles -= count;
            OpponentMarbles += count;
        }
        else if (evenOrOdd == 1 /* odd */ && count % 2 == 0)
        {
            MessageBox.Show($"Your opponent guessed odd and got it right. You lost {count} marbles.", "Opponent won this round", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            YourMarbles -= count;
            OpponentMarbles += count;
        }
        else if (evenOrOdd == 0 /* odd */)
        {
            MessageBox.Show($"Your opponent guessed odd and got it wrong. You got {count} marbles.", "Opponent won this round", MessageBoxButtons.OK, MessageBoxIcon.Information);
            YourMarbles += count;
            OpponentMarbles -= count;
        }
        else if (evenOrOdd == 1 /* even */)
        {
            MessageBox.Show($"Your opponent guessed even and got it wrong. You got {count} marbles.", "Opponent won this round", MessageBoxButtons.OK, MessageBoxIcon.Information);
            YourMarbles += count;
            OpponentMarbles -= count;
        }

        if (YourMarbles >= 20)
        {
            MessageBox.Show($"You won the game.", "Victory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            State = GameState.GameEnded;
        }
        else if (YourMarbles <= 0)
        {
            MessageBox.Show($"You lost the game.", "Loss", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
        var evenOrOdd = RandomNumberGenerator.GetInt32(1);
        if (evenOrOdd == 0 /* even */ && !isOdd)
        {
            MessageBox.Show($"Your guessed even and got it right. You got {count} marbles.", "You won this round", MessageBoxButtons.OK, MessageBoxIcon.Information);
            YourMarbles += count;
            OpponentMarbles -= count;
        }
        else if (evenOrOdd == 1 /* odd */ && isOdd)
        {
            MessageBox.Show($"Your guessed odd and got it right. You got {count} marbles.", "You won this round", MessageBoxButtons.OK, MessageBoxIcon.Information);
            YourMarbles += count;
            OpponentMarbles -= count;
        }
        else if (isOdd)
        {
            MessageBox.Show($"Your guessed odd and got it wrong since opponent placed {count} marbles. You lost {count} marbles.", "You lost this round", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            YourMarbles -= count;
            OpponentMarbles += count;
        }
        else if (!isOdd)
        {
            MessageBox.Show($"Your guessed even and got it wrong since opponent placed {count} marbles. You lost {count} marbles.", "You lost this round", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            YourMarbles -= count;
            OpponentMarbles += count;
        }

        if (YourMarbles >= 20)
        {
            MessageBox.Show($"You won the game.", "Victory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            State = GameState.GameEnded;
        }
        else if (YourMarbles <= 0)
        {
            MessageBox.Show($"You lost the game.", "Loss", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            State = GameState.GameEnded;
        }
        else
        {
            State = GameState.ChooseNumberOfMarbles;
        }
    }
}
