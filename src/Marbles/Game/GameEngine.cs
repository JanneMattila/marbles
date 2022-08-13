using System.Drawing.Drawing2D;

namespace Marbles.Game;

public class GameEngine
{
    public bool IsRunning { get; set; }
    public bool ShowDebugInfo { get; set; }
    public bool ShowMenu { get; set; } = true;

    public bool KeyUp { get; set; }
    public bool KeyDown { get; set; }
    public bool KeyLeft { get; set; }
    public bool KeyRight { get; set; }

    private Font _font = new("Segoe UI", 16, GraphicsUnit.Point);
    private Pen _gridPen = new(new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.DarkSlateGray), 1);
    private MainMenu _mainMenu;

    private DateTime _lastDraw = DateTime.Now;
    private DateTime _lastRateTime = DateTime.Now;
    private int _lastRate = 25;
    private int _rate = 0;

    public GameEngine()
    {
        _mainMenu = new MainMenu(this);
    }

    public bool KeyPress(Keys key)
    {
        if (key == Keys.F12) ShowDebugInfo = !ShowDebugInfo;
        else if (key == Keys.Escape)
        {
            ShowMenu = !ShowMenu;
            _mainMenu.ShowMenu(ShowMenu);
        }
        else if (ShowMenu)
        {
            _mainMenu.KeyPress(key);
            return false;
        }
        return true;
    }

    public async Task LoadAsync()
    {
        await _mainMenu.LoadAsync();
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(double delta)
    {
        await Task.CompletedTask;
    }

    public void Draw(Graphics graphics, Rectangle clipRectangle)
    {
        _rate++;
        var now = DateTime.Now;
        var frameRateTimeDiff = now - _lastRateTime;
        var drawTimeDiff = now - _lastDraw;
        if (frameRateTimeDiff.TotalSeconds > 1)
        {
            _lastRate = _rate;
            _lastRateTime = now;
            _rate = 0;
        }

        if (ShowMenu)
        {
            _mainMenu.Draw(graphics, clipRectangle);
        }
        else
        {
            graphics.Clear(Color.Black);

            if (!IsRunning)
            {
                graphics.DrawString($"Game paused", _font, Brushes.SlateGray, 10, 5);
                return;
            }

            graphics.DrawString($"Running", _font, Brushes.SlateGray, 10, 5);
        }

        if (ShowDebugInfo)
        {
            graphics.DrawString($"Rate: {_lastRate} - {Math.Round(drawTimeDiff.TotalMilliseconds, 2)}", _font, Brushes.SlateGray, 10, clipRectangle.Bottom - 60);
        }
        _lastDraw = DateTime.Now;
    }
}
