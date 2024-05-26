using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.GameObjects.Composed;

namespace SFML_tutorial.BaseEngine.Window.Composed;

public class GameWindow
{
    /// <summary>
    /// The underlying SFML.NET RenderWindow object
    /// </summary>
    public RenderWindow RenderWindow { get; private set; }
    /// <summary>
    /// The Color that is set when the window is cleared
    /// </summary>
    public static Color WindowBackgroundColor { get; set; } = CORNFLOWER_BLUE;
    private static readonly Color CORNFLOWER_BLUE = new(147, 204, 234);

    private readonly Clock deltaClock = new();
    // keys sorted in ascending order
    private SortedDictionary<RenderLayer, List<GameObject>> gameObjects = new(Comparer<RenderLayer>.Create((l, r) => l - r));

    private static string windowTitle;
    public static string WindowTitle
    {
        get => windowTitle;
        set => Instance.RenderWindow.SetTitle(windowTitle = value);
    }
    /// <summary>
    /// The time elapsed since the previous frame was drawn
    /// </summary>
    public static Time DeltaTime { get; private set; } = default;

    private static GameWindow? instance;
    public static GameWindow Instance => instance ??= new GameWindow();

    static GameWindow()
    {
        windowTitle = "My Window";
    }

    private GameWindow()
    {
        (uint width, uint height) = (1200, 800);
        RenderWindow = new RenderWindow(new VideoMode(width, height), windowTitle);
    }

    public static bool Contains(RenderLayer renderLayer, GameObject gameObject)
    {
        return Instance.gameObjects.ContainsKey(renderLayer) && Instance.gameObjects[renderLayer].Contains(gameObject);
    }
    public static bool Contains(GameObject gameObject)
        => Instance.gameObjects.Keys.Any(renderLayer => Contains(renderLayer, gameObject));

    public static void Add(RenderLayer renderLayer, GameObject gameObject)
    {
        if (Contains(gameObject))
        {
            throw new InvalidOperationException("GameObject was already added to window!");
        }
        if (Instance.gameObjects.TryGetValue(renderLayer, out List<GameObject>? value))
        {
            value.Add(gameObject);
        }
        else
        {
            Instance.gameObjects[renderLayer] = [gameObject];
        }
        gameObject.Attach();
    }
    public static void Add(List<(RenderLayer renderLayer, GameObject gameObject)> layeredGameObjects)
    {
        foreach (var (renderLayer, gameObject) in layeredGameObjects)
        {
            Add(renderLayer, gameObject);
        }
    }

    public static T? FindObjectOfType<T>(RenderLayer renderLayer)
    {
        foreach (var gameObject in Instance.gameObjects[renderLayer])
        {
            if (gameObject is T t)
            {
                return t;
            }
        }
        return default;
    }

    public static List<T> FindObjectsOfType<T>()
    {
        List<T> result = [];
        foreach (var gameObject in Instance.gameObjects.Keys.SelectMany(key => Instance.gameObjects[key]))
        {
            if (gameObject is T t)
            {
                result.Add(t);
            }
        }
        return result;
    }

    public static T? FindObjectOfType<T>()
    {
        foreach (var gameObject in Instance.gameObjects.Keys.SelectMany(key => Instance.gameObjects[key]))
        {
            if (gameObject is T t)
            {
                return t;
            }
        }
        return default;
    }

    public static bool TryRemove(RenderLayer renderLayer, GameObject gameObject)
        => Instance.gameObjects.ContainsKey(renderLayer) && Instance.gameObjects[renderLayer].Remove(gameObject);
    public static bool TryRemove(GameObject gameObject)
    {
        foreach (RenderLayer key in Instance.gameObjects.Keys)
        {
            if (TryRemove(key, gameObject))
            {
                return true;
            }
        }
        return false;
    }

    public static void Run()
    {
        Init();
        while (Instance.RenderWindow != null && Instance.RenderWindow.IsOpen)
        {
            DeltaTime = Instance.deltaClock.Restart();
            Update();
            HandleCollisions();
            Render();
        }
    }

    public static void Quit()
    {
        HandleQuit();
    }
    private static void Init()
    {
        Instance.RenderWindow.SetFramerateLimit(120);
        InitStandardEvents();
    }
    private static void InitStandardEvents()
    {
        // window click close
        Instance.RenderWindow.Closed += (sender, eventArgs) =>
        {
            HandleQuit();
        };
        Instance.RenderWindow.KeyPressed += (sender, keyEvent) =>
        {
            if (keyEvent.Code == Keyboard.Key.Escape)
            {
                HandleQuit();
            }
        };
        Instance.RenderWindow.Resized += (sender, sizeEvent) =>
        {
            Instance.RenderWindow.SetView(new View(new FloatRect(0, 0, sizeEvent.Width, sizeEvent.Height)));
        };
    }
    private static void Update()
    {
        Instance.RenderWindow.DispatchEvents();
        OnEachGameObject((gameObject) => gameObject.Update());
    }

    // not performant to n^2 check every gameobject for colliders but oh well
    // also this is a discrete collision system because it's just righting objects which are inside other colliders
    private static void HandleCollisions()
    {
        bool BothAreTriggers(Collider2D col1, Collider2D col2) => col1.IsTrigger && col2.IsTrigger;
        bool OneIsTrigger(Collider2D col1, Collider2D col2) => col1.IsTrigger ^ col2.IsTrigger;

        void PushCollidersApart(Collider2D col1, Collider2D col2)
        {
            if (col1.IsStatic && col2.IsStatic)
            {
                return; // neither moves
            }
            if (col1.IsStatic)
            {
                // move col2
                col2.RepositionFromCollision(col1.Bounds);
            }
            else if (col2.IsStatic)
            {
                // move col1
                col1.RepositionFromCollision(col2.Bounds);
            }
            else
            {
                Vector2f originalPos = col1.PositionableGameObject.Position;
                col1.RepositionFromCollision(col2.Bounds);
                col1.PositionableGameObject.Position -= originalPos / 2;
                col2.PositionableGameObject.Position = originalPos;
                // move both evenly
                //Vector2f distanceToMove = col1.PositionableGameObject.Position - col1.PositionableGameObject.Position.PosOfNearestEdge(col2.Bounds);
                //col1.PositionableGameObject.Position += distanceToMove / 2;
                //col2.PositionableGameObject.Position += distanceToMove / 2;
            }
        }

        OnEachGameObjectWhere((firstGameObject) => firstGameObject.Collider != null && firstGameObject.Collider.IsActive)((firstGameObject) =>
        {
            OnEachGameObjectWhere(otherGameObject => otherGameObject != firstGameObject && otherGameObject.Collider != null && otherGameObject.Collider.IsActive)((otherGameObject) =>
            {
                // if no collision between this object and another but the other is in the first object's colliding with list, call on collision exit and pop the collider

                if (firstGameObject.Collider!.Bounds.Intersects(otherGameObject.Collider!.Bounds))
                {
                    if (BothAreTriggers(firstGameObject.Collider, otherGameObject.Collider))
                    {
                        return;
                    }
                    if (firstGameObject.Collider.CollidingWith.Contains(otherGameObject))
                    {
                        if (OneIsTrigger(firstGameObject.Collider, otherGameObject.Collider))
                        {
                            firstGameObject.OnTriggerStay2D(otherGameObject.Collider);
                        }
                        else // already handled the both are triggers case at the top in the early return
                        {
                            firstGameObject.OnCollisionStay2D(otherGameObject.Collider);
                            otherGameObject.OnCollisionStay2D(firstGameObject.Collider);
                            PushCollidersApart(firstGameObject.Collider, otherGameObject.Collider);
                        }
                    }
                    else
                    {
                        if (OneIsTrigger(firstGameObject.Collider, otherGameObject.Collider))
                        {
                            firstGameObject.OnTriggerEnter2D(otherGameObject.Collider);
                            firstGameObject.Collider.CollidingWith.Add(otherGameObject.Collider.PositionableGameObject);
                        }
                        else // already handled the both are triggers case at the top in the early return
                        {
                            firstGameObject.OnCollisionEnter2D(otherGameObject.Collider);
                            otherGameObject.OnCollisionEnter2D(firstGameObject.Collider);
                            firstGameObject.Collider.CollidingWith.Add(otherGameObject.Collider.PositionableGameObject);
                            otherGameObject.Collider.CollidingWith.Add(firstGameObject.Collider.PositionableGameObject);
                            PushCollidersApart(firstGameObject.Collider, otherGameObject.Collider);
                        }
                    }
                }
                else
                {
                    if (firstGameObject.Collider.CollidingWith.Contains(otherGameObject))
                    {
                        // we don't care if only one is a trigger, if both became triggers then we need to pop all collisions and trigger exit
                        if (!otherGameObject.Collider.IsTrigger)
                        {
                            firstGameObject.OnCollisionExit2D(otherGameObject.Collider);
                            otherGameObject.OnCollisionExit2D(firstGameObject.Collider);
                        }
                        else
                        {
                            firstGameObject.OnTriggerExit2D(otherGameObject.Collider);
                        }
                        firstGameObject.Collider.CollidingWith.Remove(otherGameObject.Collider.PositionableGameObject);
                    }
                }
            });
        });
    }

    private static void Render()
    {
        Instance.RenderWindow.Clear(WindowBackgroundColor);
        OnEachGameObject((gameObject) => gameObject.Drawables.ForEach((drawable) => Instance.RenderWindow.Draw(drawable)));
        Instance.RenderWindow.Display();
    }

    private static List<GameObject> ActiveGameObjects => Instance.gameObjects.Keys.SelectMany(key => Instance.gameObjects[key]).Where(gameObject => gameObject.IsActive).ToList();

    // layers should be iterated over in the correct order due to the SortedDictionary calling Render on lower layers first
    /// <summary>
    /// Calls doOnEach for each GameObject in Instance.gameObjects in order of keys 
    /// with respect to the sorting method given to Instance.gameObjects SortedDictionary.
    /// </summary>
    /// <param name="doOnEach">The callback function taking the GameObject to perform an action on</param>
    private static void OnEachGameObject(Action<GameObject> doOnEach)
    {
        ActiveGameObjects.ForEach(doOnEach);
    }

    private static Action<Action<GameObject>> OnEachGameObjectWhere(Func<GameObject, bool> predicate)
    {
        List<GameObject> gameObjects = ActiveGameObjects.Where(predicate).ToList();
        return gameObjects.ForEach;
    }

    private static void HandleQuit()
    {
        Console.WriteLine("closed window");
        Instance.RenderWindow.Close();
    }
}
