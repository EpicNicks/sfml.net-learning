using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.Breakout.Entities;
using SFML_tutorial.Games.Breakout.UI;
using SFML_tutorial.Games.PongGame.StateManagers;
using SFML_tutorial.BaseEngine.GameObjects.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.Properties;

namespace SFML_tutorial.Games.Breakout;
public static class BreakoutMain
{
    public const int GAME_WIDTH = 1200;
    public const int GAME_HEIGHT = 800;
    public static void Run()
    {
        GameWindow.WindowTitle = "Breakout";

        GameWindow.AddScene(SetupLevel());
        GameWindow.AddScene(Level1());
        GameWindow.AddScene(Level2());

        GameWindow.AddScene(VictoryScreen());

        GameWindow.Run();
    }

    private static (RenderLayer, GameObject)[] LevelObjects()
    {
        return
        [
            // walls
            (RenderLayer.BASE, new ColliderWall(new(0, -100, GAME_WIDTH, 100), true)), // top wall
            (RenderLayer.BASE, new ColliderWall(new(GAME_WIDTH, 0, 100, GAME_HEIGHT), true)), // right wall
            (RenderLayer.BASE, new ColliderWall(new(-100, 0, 100, GAME_HEIGHT), true)), // left wall
            (RenderLayer.BASE, new FailureTrigger(new(0, GAME_HEIGHT, GAME_WIDTH, 100), true)), // bottom trigger

            (RenderLayer.BASE, new Ball
            {
                MoveSpeed = 500f
            }),

            (RenderLayer.UI, new TriesText(5)
            {
                Position = new(-50, -50),
            })
        ];
    }

    // creates all persistent objects and immediately goes to the next scene
    private static Scene SetupLevel()
    {
        return new Scene("__setup", add =>
        {
            add((RenderLayer.NONE, new BrickManager
            {
                PersistanceInfo = new GameObject.Persistance
                {
                    persistId = 8008135L,
                    persistOnSceneTransition = true,
                }
            }));
            add((RenderLayer.BASE, new PlayerPaddle(new(600 - 50, 800 - 50, 100, 20))
            {
                MoveSpeed = 500,
                PersistanceInfo = new GameObject.Persistance
                {
                    persistId = 12345L,
                    persistOnSceneTransition = true,
                }
            }));
            add((RenderLayer.UI, new ScoreText
            {
                Position = new(0, 50),
                PersistanceInfo = new GameObject.Persistance
                {
                    persistId = 121212121L,
                    persistOnSceneTransition = true,
                }
            }));
            GameWindow.LoadNextScene();
        });
    }

    private static Scene Level1()
    {
        return new Scene("Level 1", add =>
        {
            add(LevelObjects());
            GenerateFloatRectRow(10, 200, 300, 20, new Vector2f(50, 20)).ForEach((bounds) =>
            {
                add((RenderLayer.BASE, new Brick(bounds, 1, 1)));
            });
            GenerateFloatRectRow(7, 300, 200, 20, new Vector2f(50, 20)).ForEach((bounds) =>
            {
                add((RenderLayer.BASE, new Brick(bounds, 2, 3)));
            });
        });
    }

    private static Scene Level2()
    {
        return new Scene("Level 2", add =>
        {
            add(LevelObjects());
            GenerateFloatRectRow(15, 100, 300, 20, new Vector2f(50, 20)).ForEach((bounds) =>
            {
                add((RenderLayer.BASE, new Brick(bounds, 1, 1)));
            });
            GenerateFloatRectRow(10, 200, 200, 20, new Vector2f(50, 20)).ForEach((bounds) =>
            {
                add((RenderLayer.BASE, new Brick(bounds, 2, 3)));
            });
            GenerateFloatRectRow(7, 300, 100, 20, new Vector2f(50, 20)).ForEach((bounds) =>
            {
                add((RenderLayer.BASE, new Brick(bounds, 3, 5)));
            });

            Ball? ball = GameWindow.FindObjectOfType<Ball>();
            if (ball != null)
            {
                ball.MoveSpeed = 800;
            }
        });
    }

    private static Scene VictoryScreen()
    {
        return new Scene("Victory Screen", add =>
        {
            Text drawableText = new Text
            {
                DisplayedString = "YOU WIN",
                CharacterSize = 48,
                FillColor = Color.White,
                Font = new(Resources.Roboto_Black)
            };
            add((RenderLayer.UI, new Positionable
            {
                Drawables = [drawableText],
                Position = new(GameWindow.Instance.UiView.Size.X / 2f - drawableText.GetLocalBounds().Width / 2f, GameWindow.Instance.UiView.Size.Y / 2f - drawableText.GetLocalBounds().Height / 2f),
            }));
        });
    }
    private static List<FloatRect> GenerateFloatRectRow(int amount, float startingXPos, float yPos, float spaceBetween, Vector2f size)
    {
        List<FloatRect> output = [];
        for (int i = 0; i < amount; i++)
        {
            output.Add(new FloatRect(new(startingXPos + i * (size.X + spaceBetween), yPos), size));
        }
        return output;
    }
}
