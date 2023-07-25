namespace BoxClient.Game;

public class MainMenu
{
    private readonly GameEngine _gameEngine;
    private Font _font = new("Segoe UI", 16, GraphicsUnit.Point);
    private int _selectedMenu = 0;

    public MainMenu(GameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public void ShowMenu(bool show)
    {
        _selectedMenu = 0;
    }

    public void KeyPress(Keys key)
    {
        if (key == Keys.Enter || key == Keys.Space)
        {
            _gameEngine.ShowMenu = false;
        }
        else if (key == Keys.Up || key == Keys.W)
        {
            _selectedMenu = Math.Abs(_selectedMenu - 1);
        }
        else if (key == Keys.Down || key == Keys.S)
        {
            _selectedMenu = (_selectedMenu + 1) % 2;
        }
    }

    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }

    public void Draw(Graphics graphics, Rectangle clipRectangle)
    {
        graphics.FillRectangle(Brushes.LightBlue, clipRectangle);
        graphics.DrawString("Menu", _font, Brushes.Black, 10, 5);

        graphics.FillRectangle(_selectedMenu == 0 ? Brushes.DarkBlue : Brushes.LightBlue, 0, 100, clipRectangle.Width, 100);
        graphics.DrawString("Play Online", _font, _selectedMenu == 0 ? Brushes.White : Brushes.Black, 100, 120);

        graphics.FillRectangle(_selectedMenu == 1 ? Brushes.DarkBlue : Brushes.LightBlue, 0, 200, clipRectangle.Width, 100);
        graphics.DrawString("Play Local", _font, _selectedMenu == 1 ? Brushes.White : Brushes.Black, 100, 220);
    }
}
