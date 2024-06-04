using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;
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
            GenerateFloatRectRow(10, 200, 300, 20, new Vector2f(50, 20)).ForEach((bounds) =>
            {
                add((RenderLayer.BASE, new Brick(bounds, Color.Red, 1, 1)));
            });
            add((RenderLayer.UI, new ScoreText { Position = new(0, 50) }));
            add((RenderLayer.BASE, new PlayerPaddle(new(600 - 50, 800 - 50, 100, 20)) { MoveSpeed = 500 }));
        }));

        GameWindow.Run();
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
