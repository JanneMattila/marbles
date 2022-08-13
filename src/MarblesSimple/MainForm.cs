namespace MarblesSimple;

public partial class MainForm : Form
{
    private Game _game = new Game();

    public MainForm()
    {
        InitializeComponent();
    }

    private void PlayLocalButton_Click(object sender, EventArgs e)
    {
        _game.StartGame();

        UpdateControlEnableState();
    }

    private void SelectButton_Click(object sender, EventArgs e)
    {
        var num = (int)SelectMarblesNum.Value;
        _game.SelectMarbles(num);
        UpdateControlEnableState();
    }

    private void OddButton_Click(object sender, EventArgs e)
    {
        _game.SelectOddOrEvent(isOdd: true);
        UpdateControlEnableState();
    }

    private void EvenButton_Click(object sender, EventArgs e)
    {
        _game.SelectOddOrEvent(isOdd: false);
        UpdateControlEnableState();
    }

    private void UpdateControlEnableState()
    {
        SelectMarblesNum.Value = SelectMarblesNum.Maximum = _game.YourMarbles;
        YouHaveMarblesLeftLabel.Text = $"You have {_game.YourMarbles} marbles left. How many will you choose?";

        GroupSelectMarbles.Enabled = _game.State == GameState.ChooseNumberOfMarbles;
        GroupOddOrEven.Enabled = _game.State == GameState.ChooseOddOrEven;
        StatusLabel.Text = _game.State == GameState.WaitingForOpponent ? "Waiting for opponent" : "";
    }
}