using SFML_tutorial.BaseEngine.Window.Composed;

using SFML_tutorial.Games.PongGame.UI;
using SFML_tutorial.Games.PongGame.Entities;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

using SFML.System;
using SFML.Window;

namespace SFML_tutorial.Games.PongGame;

public class PongMain
{
    public static void Run()
    {
        GameWindow.WindowTitle = "Pong";
        // init general events for the game

        GameWindow.Add(RenderLayer.UI, new ScoreText
        {
            Position = new Vector2f(-20, 20),
            Anchors = (UIAnchored.UIAnchor.CENTER, UIAnchored.UIAnchor.START)
        });
        GameWindow.Add(RenderLayer.NONE, new ColliderWall { IsTopWall = true });
        GameWindow.Add(RenderLayer.NONE, new ColliderWall { IsTopWall = false });
        GameWindow.Add(RenderLayer.NONE, new ScoreTriggerWall(true));
        GameWindow.Add(RenderLayer.NONE, new ScoreTriggerWall(false));
        GameWindow.Add(RenderLayer.BASE, new Ball
        {
            MoveSpeed = 500f,
        });
        GameWindow.Add(RenderLayer.BASE, new PlayerPaddle
        (
            [Keyboard.Key.Up, Keyboard.Key.W],
            [Keyboard.Key.Down, Keyboard.Key.S],
            new Vector2f(20, 100)
        )
        {
            MoveSpeed = 700f,
            IsLeftSidePlayer = true,
        });
        GameWindow.Add(RenderLayer.BASE, new AIPaddle(new Vector2f(20, 100))
        {
            MoveSpeed = 320f,
            IsLeftSidePlayer = false,
        });

        GameWindow.Run();
    }
}
