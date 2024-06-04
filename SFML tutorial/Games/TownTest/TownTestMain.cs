using SFML.System;
using SFML.Graphics;

using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.TownTest.Entities;
using SFML_tutorial.Games.TownTest.UI;

namespace SFML_tutorial.Games.TownTest;
/// <summary>
/// Mostly just a test of World space and camera follow working and a sandbox to fix it in
/// </summary>
public class TownTestMain
{
    public static void Run()
    {
        Vector2f origin = new Vector2f(0, 0);

        GameWindow.Instance.MainView.Move(origin);

        GameWindow.AddScene(new Scene("town test main", (add) =>
        {
            add((RenderLayer.UI, new DialogueText
            {
                DisplayString = "Hello World",
                Position = new Vector2f(0, -30)
            }));
            add((RenderLayer.BASE, new Player 
            { 
                Position = new Vector2f(0, 0),
                Size = new Vector2f(20, 40),
                MoveSpeed = 500.0f
            }));
            add((RenderLayer.BASE, new BoxWall
            { 
                Color = Color.Red,
                Position = new Vector2f(100, 100),
                Size = new Vector2f(100, 100)
            }));
        }));

        GameWindow.Run();
    }
}
