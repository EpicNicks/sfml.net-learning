using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState;

namespace SFML_tutorial.BaseEngine.Window.ExternalState;

/// <summary>
/// Singleton class for the game window
/// </summary>
public class GameWindow
{
    private readonly Clock deltaClock = new Clock();
    private readonly List<IGameObject<Drawable>> gameObjects = [];
    private readonly List<IGameObject<Drawable>> uiElements = [];

    public RenderWindow RenderWindow { get; private set; }
    public string WindowName { get; private set; }
    public (uint width, uint height) Dimensions { get; private set; }
    public Time DeltaTime { get; private set; } = default;

    private GameWindow()
    {
        Dimensions = (800, 600);
        WindowName = "Game Window";
        RenderWindow = new RenderWindow(new VideoMode(Dimensions.width, Dimensions.height), "My Window");
    }

    private static GameWindow? instance;
    public static GameWindow Instance => instance ??= new GameWindow();


    private void HandleQuit()
    {
        Console.WriteLine("closed window");
        RenderWindow?.Close();
    }

    private void InitStandardEvents()
    {
        // window click close
        RenderWindow.Closed += (sender, eventArgs) =>
        {
            HandleQuit();
        };
        RenderWindow.KeyPressed += (sender, keyEvent) =>
        {
            if (keyEvent.Code == Keyboard.Key.Escape)
            {
                HandleQuit();
            }
        };
        RenderWindow.Resized += (sender, sizeEvent) =>
        {
            RenderWindow.SetView(new View(new FloatRect(0, 0, sizeEvent.Width, sizeEvent.Height)));
        };
    }

    public bool RemoveFromWindow(IGameObject<Drawable> gameObject)
    {
        if (!RemoveGameObject(gameObject))
        {
            return RemoveUIElement(gameObject);
        }
        return false;
    }

    public G AddUIElement<G>(G uiElement) where G : IGameObject<Drawable>
    {
        uiElements.Add(uiElement);
        uiElement.Attach();
        return uiElement;
    }
    public bool RemoveUIElement(IGameObject<Drawable> uiElement)
    {
        uiElement.Detach();
        return uiElements.Remove(uiElement);
    }

    public G AddGameObject<G>(G gameObject) where G : IGameObject<Drawable>
    {
        gameObjects.Add(gameObject);
        gameObject.Attach();
        return gameObject;
    }
    public bool RemoveGameObject(IGameObject<Drawable> gameObject)
    {
        gameObject.Detach();
        return gameObjects.Remove(gameObject);
    }

    private void Init()
    {
        RenderWindow.SetFramerateLimit(120);
        InitStandardEvents();
    }

    public void Update()
    {
        RenderWindow.DispatchEvents();
        foreach (var gameObject in gameObjects)
        {
            gameObject.Update();
        }
        foreach (var uiElement in uiElements)
        {
            uiElement.Update();
        }
    }

    public void Render()
    {
        RenderWindow.Clear(Color.Blue);
        foreach (var gameObject in gameObjects)
        {
            RenderWindow.Draw(gameObject.Drawable);
        }
        foreach (var uiElement in uiElements)
        {
            RenderWindow.Draw(uiElement.Drawable);
        }
        RenderWindow.Display();
    }

    public void Run()
    {
        Init();
        while (RenderWindow != null && RenderWindow.IsOpen)
        {
            DeltaTime = deltaClock.Restart();
            Update();
            Render();
        }
    }
}
