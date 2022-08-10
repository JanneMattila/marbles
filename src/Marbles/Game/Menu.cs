namespace Marbles.Game;

public class Menu
{
    private Font _font = new("Segoe UI", 16, GraphicsUnit.Point);

    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }

    public void Draw(Graphics graphics, Rectangle clipRectangle)
    {
        graphics.FillRectangle(Brushes.LightBlue, clipRectangle);
        graphics.DrawString("Menu", _font, Brushes.Black, 10, 5);
    }
}
