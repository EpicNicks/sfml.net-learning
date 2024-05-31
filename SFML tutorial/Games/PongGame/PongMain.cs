using SFML_tutorial.BaseEngine.Window.Composed;

using SFML_tutorial.Games.PongGame.UI;
using SFML_tutorial.Games.PongGame.Entities;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.GameObjects.Composed;

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
        
        // Pong is effectively a screen-space game and as such it makes sense to just throw everything on the UI layer
        GameWindow.AddScene(new Scene("main game", (add) =>
            add
            (
                (RenderLayer.UI, new ScoreText
                {
                    Position = new Vector2f(-20, 20),
                    Anchors = (UIAnchored.UIAnchor.CENTER, UIAnchored.UIAnchor.START),
                    PersistanceInfo = new GameObject.Persistance(true, 7331L)
                }),
                (RenderLayer.NONE, new ColliderWall { IsTopWall = true }),
                (RenderLayer.NONE, new ColliderWall { IsTopWall = false }),
                (RenderLayer.NONE, new ScoreTriggerWall(true)),
                (RenderLayer.NONE, new ScoreTriggerWall(false)),
                (RenderLayer.UI, new Ball { MoveSpeed = 500f }),
                (RenderLayer.UI, new PlayerPaddle
                (
                    [Keyboard.Key.Up, Keyboard.Key.W],
                    [Keyboard.Key.Down, Keyboard.Key.S],
                    new Vector2f(20, 100)
                )
                {
                    MoveSpeed = 700f,
                    IsLeftSidePlayer = false,
                }),
                (RenderLayer.UI, new AIPaddle(new Vector2f(20, 100))
                {
                    MoveSpeed = 320f,
                    IsLeftSidePlayer = true,
                })
            )
        ));

        GameWindow.AddScene(new Scene("main menu", (add) =>
        {
        }));

        GameWindow.Run();
    }
}
