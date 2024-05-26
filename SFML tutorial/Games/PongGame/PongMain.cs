using SFML_tutorial.BaseEngine.Window.Composed;

using SFML_tutorial.Games.PongGame.UI;
using SFML_tutorial.Games.PongGame.Entities;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace SFML_tutorial.Games.PongGame;

public class PongMain
{
    public static void Run()
    {
        Vector2u windowSize = GameWindow.Instance.RenderWindow.Size;
        GameWindow.WindowTitle = "Pong";

        var player1 = new PlayerPaddle([Keyboard.Key.Up, Keyboard.Key.W], [Keyboard.Key.Down, Keyboard.Key.S],new FloatRect(windowSize.X / 8, windowSize.Y / 2, 20, 100))
        {
            MoveSpeed = 700,
        };
        var player2 = new AIPaddle(new FloatRect(windowSize.X * 7f / 8f, windowSize.Y / 2, 20, 100))
        {
            MoveSpeed = 300,
        };
        var ball = new Ball(player1, player2)
        {
            MoveSpeed = 500f
        };

        GameWindow.Add(RenderLayer.UI, new ScoreText
        {
            Position = new Vector2f(-20, 20),
            Anchors = (UIAnchored.UIAnchor.CENTER, UIAnchored.UIAnchor.START)
        });
        // Top collider (positioned at the top edge, with full width minus the side colliders)
        GameWindow.Add(RenderLayer.NONE, new ColliderWall(new FloatRect(10, 0, windowSize.X - 20, 10)));
        // Bottom collider (positioned at the bottom edge, with full width minus the side colliders)
        GameWindow.Add(RenderLayer.NONE, new ColliderWall(new FloatRect(10, windowSize.Y - 10, windowSize.X - 20, 10)));
        // Left collider (positioned at the left edge, with full height)
        GameWindow.Add(RenderLayer.NONE, new ScoreTriggerWall(ScoreText.PlayerId.two, new FloatRect(0, 0, 10, windowSize.Y)));
        // Right collider (positioned at the right edge, with full height)
        GameWindow.Add(RenderLayer.NONE, new ScoreTriggerWall(ScoreText.PlayerId.one, new FloatRect(windowSize.X - 10, 0, 10, windowSize.Y)));
        GameWindow.Add(RenderLayer.BASE, ball);
        GameWindow.Add(RenderLayer.BASE, player1);
        GameWindow.Add(RenderLayer.BASE, player2);

        GameWindow.Run();
    }
}
