using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Stateful;
using SFML_tutorial.BaseEngine.CoreLibs.StateDict;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState.StateDict;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState.Stateful;
using ExternalStateGameWindow = SFML_tutorial.BaseEngine.Window.ExternalState.GameWindow;
using GameWindow = SFML_tutorial.BaseEngine.Window.Composed.GameWindow;
using SFML_tutorial.Game.Components;
using SFML_tutorial.Game.ComposedObjects;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.BaseEngine.GameObjects.Composed;
using SFML_tutorial.Games.PongGame;

namespace SFML_tutorial;

public class Program
{
    // after working through a similar system in Java, passing a state type to be consumed ends up being a roundabout way of having additional state,
    // since the drawable itself already contains much of its own state
    // since the canvas takes a drawable, the most sensible thing to do is to just have either
    //  1. wrapper classes all deriving an interface which returns a T where T : Drawable for the renderwindow to use and the rest (attach, update) for the canvas
    //      to call at the appropriate lifecycle stages
    //  2. extended classes which inherit the target drawable to provide the extra state as well as containers for composing multiple drawables with a shared state such as
    //      health bars (2 rects with one that actually changes)


    public static void Main()
    {
        PongMain.Run();
    }

    private static void RunComposedVersion()
    {
        Console.WriteLine("Composed Version Demo Running...");

        GameWindow.Add(RenderLayer.UI, new HealthBar(100, new(200, 20)) 
        {
            Position = new(-50, -50),
            DepleteRight = true,
            Anchors = (UIAnchored.UIAnchor.END, UIAnchored.UIAnchor.END),
        });
        GameWindow.Add(RenderLayer.BASE, new MoveableBox(new Vector2f(20, 100))
        { 
            MoveSpeed = 500.0f
        });
        Obstacle obstacle = new Obstacle(new FloatRect(new Vector2f(200, 200), new Vector2f(100, 100)))
        {
            MoveSpeed = 100,
        };
        obstacle.Collider.IsStatic = false;
        GameWindow.Add(RenderLayer.BASE, obstacle);

        Console.WriteLine($"Render window size {GameWindow.Instance.RenderWindow.Size}");


        GameWindow.Run();
    }


    private static void RunExternalStateVersion()
    {
        Console.WriteLine("External State Version Demo Running...");

        ExternalStateGameWindow window = ExternalStateGameWindow.Instance;
        window.RenderWindow.Size = new Vector2u(1200, 800);

        var statefulUiText = window.AddUIElement(new StatefulUIText<UITextState>("Hello, Stateful World", (50, -50))
        {
            State = new UITextState(0),
            UITextAnchors = (StatefulUIText<UITextState>.UITextAnchor.START, StatefulUIText<UITextState>.UITextAnchor.END),
            OnUpdate = (uiText) =>
            {
                uiText.Text.DisplayedString = (++uiText.State.Count) + " stateful";
            }
        });

        var uiText = window.AddUIElement(new UIText("Hello, World", (0, 0))
        {
            ["count"] = 0,
            OnAttach = (uiText) =>
            {
            },
            OnUpdate = (uiText) =>
            {
                uiText.SetState("count", (int count) =>
                {
                    uiText.Text.DisplayedString = (++count).ToString();
                    return count;
                });
            },
        });
        HPComponent? hp = uiText.AddComponent<HPComponent>();
        if (hp != null)
        {
            hp.MaxHp = 100;
            hp.CurHp = 100;
        }

        window.AddUIElement(new UIText("0", (-10, 10))
        {
            OnAttach = (dtDisplay) => { },
            UITextAnchors = (UIText.UITextAnchor.END, UIText.UITextAnchor.START),
            OnUpdate = (dtDisplay) =>
            {
                dtDisplay.Text.DisplayedString = $"delta time: {window.DeltaTime.AsSeconds()}";
            },
        });

        window.AddGameObject(new GenericGameObject<RectangleShape>(new RectangleShape(new Vector2f(100, 100))
        {
            FillColor = Color.Red,
            OutlineColor = new Color(1, 1, 1, 255),
            OutlineThickness = 1,
        })
        {
            ["velocity"] = new Vector2f(0, 0),
            OnAttach = (rect) =>
            {
                rect.Drawable.Position = new Vector2f(rect.Drawable.Size.X * 0.5f, 0);
            },
            OnUpdate = (rect) =>
            {
                // basic gravity, might be able to modularize by having inherited functions for physics objects that apply this over a custom State field "velocity"
                if ((rect.Drawable.Position.Y + rect.Drawable.Size.Y) < window.RenderWindow.Size.Y)
                {
                    Vector2f velocity = (Vector2f)rect["velocity"]!;
                    velocity.Y += 9.81f * window.DeltaTime.AsSeconds();
                    rect.Drawable.Position += velocity;
                    rect["velocity"] = velocity;
                }
                else
                {
                    rect["velocity"] = new Vector2f(0, 0);
                    rect.Drawable.Position = new Vector2f(rect.Drawable.Position.X, window.RenderWindow.Size.Y - rect.Drawable.Size.Y);
                }
            }
        });


        window.AddGameObject(new GenericStatefulGameObject<RectangleShape, RectangleShapeObjectState>(new RectangleShape(new Vector2f(200, 200))
        {
            FillColor = Color.Green,
            OutlineColor = new Color(1, 1, 1, 255),
            OutlineThickness = 1,
        })
        {
            State = new RectangleShapeObjectState(new Vector2f(0, 0)),
            OnAttach = (rect) =>
            {
                rect.Drawable.Position = new Vector2f(rect.Drawable.Size.X * 1.5f, 0);
            },
            OnUpdate = (rect) =>
            {
                if ((rect.Drawable.Position.Y + rect.Drawable.Size.Y) < window.RenderWindow.Size.Y)
                {
                    rect.State.Velocity.Y += 9.81f * window.DeltaTime.AsSeconds();
                    rect.Drawable.Position += rect.State.Velocity;
                }
                else
                {
                    rect.State.Velocity = new Vector2f(0, 0);
                    rect.Drawable.Position = new Vector2f(rect.Drawable.Position.X, window.RenderWindow.Size.Y - rect.Drawable.Size.Y);
                }
            }
        }); ;


        window.Run();
    }
    class RectangleShapeObjectState(Vector2f velocity)
    {
        public Vector2f Velocity = velocity;
    }

    class UITextState(int count)
    {
        public int Count = count;
    }

}
