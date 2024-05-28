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

        GameWindow.Instance.RenderWindow.KeyPressed += (_, keyEvent) =>
        {
            if (keyEvent.Code == Keyboard.Key.M)
            {
                if (GameWindow.LoadedSceneName == "main game")
                {
                    GameWindow.LoadScene("main menu");
                }
                else if (GameWindow.LoadedSceneName == "main menu")
                {
                    GameWindow.LoadScene("main game");
                }
            }
        };

        GameWindow.AddScene(new Scene("main game", (add) =>
        {
            add(RenderLayer.UI, new ScoreText
            {
                Position = new Vector2f(-20, 20),
                Anchors = (UIAnchored.UIAnchor.CENTER, UIAnchored.UIAnchor.START)
            });
            add(RenderLayer.NONE, new ColliderWall { IsTopWall = true });
            add(RenderLayer.NONE, new ColliderWall { IsTopWall = false });
            add(RenderLayer.NONE, new ScoreTriggerWall(true));
            add(RenderLayer.NONE, new ScoreTriggerWall(false));
            add(RenderLayer.BASE, new Ball
            {
                MoveSpeed = 500f,
            });
            add(RenderLayer.BASE, new PlayerPaddle
            (
                [Keyboard.Key.Up, Keyboard.Key.W],
                [Keyboard.Key.Down, Keyboard.Key.S],
                new Vector2f(20, 100)
            )
            {
                MoveSpeed = 700f,
                IsLeftSidePlayer = true,
            });
            add(RenderLayer.BASE, new AIPaddle(new Vector2f(20, 100))
            {
                MoveSpeed = 320f,
                IsLeftSidePlayer = false,
            });
        }));

        GameWindow.AddScene(new Scene("main menu", (add) =>
        {
            add(RenderLayer.UI, new ScoreText());
        }));

        GameWindow.Run();
    }
}
