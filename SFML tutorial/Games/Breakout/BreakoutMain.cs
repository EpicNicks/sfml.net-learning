using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.Breakout.Entities;
using SFML_tutorial.Games.Breakout.UI;

namespace SFML_tutorial.Games.Breakout;
public static class BreakoutMain
{
    public static void Run()
    {
        GameWindow.WindowTitle = "Breakout";

        GameWindow.AddScene(new Scene("Level 1", add =>
        {
            add((RenderLayer.UI, new ScoreText { Position = new(0, 50) }));
            add((RenderLayer.BASE, new PlayerPaddle(new(600 - 50, 800 - 50, 100, 20)) { MoveSpeed = 500 }));
        }));

        GameWindow.Run();
    }
}
