using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.GameObjects.Composed;

namespace SFML_tutorial.BaseEngine.Window.Composed;

public class GameWindow
{
    /// <summary>
    /// Used by GameObjects not written to the UI or NONE RenderLayers.
    /// Move around to move the "main camera".
    /// </summary>
    public View MainView { get; private set; }
    /// <summary>
    /// Used by GameObjects written to the UI RenderLayer.
    /// Not really meant to be moved about like MainView but can be.
    /// </summary>
    public View UiView { get; private set; }

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
    private SortedDictionary<RenderLayer, List<GameObject>> GameObjects
    {
        // TODO: handle scene loading next
        get => LoadedScene?.GameObjects ?? [];
    }

    private List<Scene> sceneList = [];
    private int curSceneIndex = 0;
    // TODO - Unhandled: Initialization when there are no scenes in the list
    // same goes for Add, TryRemove, and all others
    private Scene? LoadedScene => curSceneIndex < sceneList.Count ? sceneList[curSceneIndex] : null;

    public Queue<GameObject> AttachQueue { get; private set; } = [];

    private static string windowTitle;
    public static string WindowTitle
    {
        get => windowTitle;
        set => Instance.RenderWindow.SetTitle(windowTitle = value);
    }
    /// <summary>
    /// A shortener for the common Instance.RenderWindow.Size get
    /// </summary>
    public static Vector2u Size => Instance.RenderWindow.Size;
    /// <summary>
    /// The Aspect Ratio of the window (Width / Height)
    /// </summary>
    public static float AspectRatio => Size.X / Size.Y;
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
        MainView = new View(new FloatRect(0, 0, width, height))
        {
            Viewport = new FloatRect(0, 0, 1, 1)
        };
        UiView = RenderWindow.DefaultView;
    }

    public static bool Contains(RenderLayer renderLayer, GameObject gameObject) => Instance.LoadedScene?.Contains(renderLayer, gameObject) ?? false;
    public static bool Contains(GameObject gameObject) => Instance.LoadedScene?.Contains(gameObject) ?? false;
    public static void Add(RenderLayer renderLayer, GameObject gameObject)
    {
        if (Instance.LoadedScene == null)
        {
            Console.Error.WriteLine("No scene was loaded to add the provided GameObject to");
        }
        Instance.LoadedScene?.Add(renderLayer, gameObject);
    }
    public static void Add(List<(RenderLayer renderLayer, GameObject gameObject)> layeredGameObjects)
    {
        if (Instance.LoadedScene == null)
        {
            Console.Error.WriteLine("No scene was loaded to add the provided GameObject to");
        }
        Instance.LoadedScene?.Add(layeredGameObjects);
    }
    public static List<T> FindObjectsOfType<T>() => Instance.LoadedScene?.FindObjectsOfType<T>() ?? [];
    public static T? FindObjectOfType<T>() => Instance.LoadedScene is not null ? Instance.LoadedScene.FindObjectOfType<T>() : default;
    public static T? FindObjectOfType<T>(RenderLayer renderLayer) => Instance.LoadedScene is not null ? Instance.LoadedScene.FindObjectOfType<T>(renderLayer) : default;
    public static bool TryRemove(RenderLayer renderLayer, GameObject gameObject) => Instance.LoadedScene?.TryRemove(renderLayer, gameObject) ?? false;
    public static bool TryRemove(GameObject gameObject) => Instance.LoadedScene?.TryRemove(gameObject) ?? false;

    public static string? LoadedSceneName => Instance.LoadedScene?.Name;

    public static void AddScene(Scene scene)
    {
        if (Instance.sceneList.Any(s => s.Name == scene.Name)) // Contains(scene) case logically covered here
        {
            throw new InvalidOperationException($"Scene with name: {scene} was already in the list");
        }
        Instance.sceneList.Add(scene);
    }

    public static void LoadNextScene()
    {
        if (Instance.LoadedScene is null)
        {
            throw new InvalidOperationException("There is no scene loaded currently of which the next should be loaded.");
        }
        if (Instance.curSceneIndex + 1 >= Instance.sceneList.Count)
        {
            throw new InvalidOperationException("There was no next scene. Index is at or greater than Count of Scene List");
        }
        var persistentGameObjects = Instance.LoadedScene?.Unload() ?? [];
        Instance.curSceneIndex++;
        Instance.LoadedScene?.Init(persistentGameObjects);
    }

    public static void LoadScene(string name)
    {
        int namedSceneIndex = Instance.sceneList.FindIndex(scene => scene.Name == name);
        if (namedSceneIndex == -1)
        {
            throw new InvalidOperationException($"No scene was found in the Scene List with the name: {name}");
        }
        var persistentGameObjects = Instance.LoadedScene?.Unload() ?? [];
        Instance.curSceneIndex = namedSceneIndex;
        Instance.LoadedScene?.Init(persistentGameObjects);
    }

    public static void Run()
    {
        Init();
        while (Instance.RenderWindow != null && Instance.RenderWindow.IsOpen)
        {
            DeltaTime = Instance.deltaClock.Restart();
            ProcessAttachQueue();
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
        if (Instance.LoadedScene is null)
        {
            Console.Error.WriteLine("No initial scene to load provided to GameWindow. Consider calling AddScene with a Scene to load.");
        }
        else
        {
            Instance.LoadedScene.Init([]);
        }
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
            // Update the viewport of the main view to maintain aspect ratio or full scale
            Instance.MainView.Size = new Vector2f(sizeEvent.Width, sizeEvent.Height);  // Optional: maintain aspect ratio
            Instance.MainView.Viewport = new FloatRect(0, 0, 1, 1);  // Always render the world view across the entire window

            // Update the UI view to match the new window size for direct screen space mapping
            Instance.UiView.Reset(new FloatRect(0, 0, sizeEvent.Width, sizeEvent.Height));

            // Update the window's view to the modified main view after resizing
            Instance.RenderWindow.SetView(Instance.MainView);
        };
    }

    private static void ProcessAttachQueue()
    {
        while (Instance.AttachQueue.Count > 0)
        {
            GameObject dequeuedGameObject = Instance.AttachQueue.Dequeue();
            if (dequeuedGameObject.IsActive)
            {
                dequeuedGameObject.Attach();
            }
        }
    }

    private static void Update()
    {
        Instance.RenderWindow.DispatchEvents();
        OnEachGameObject((gameObject) => gameObject.Update());
    }

    // not performant to n^2 check every gameobject for colliders but oh well
    // also this is a discrete collision system because it's just righting objects which are inside other colliders
    // TODO: extract out to Collision Engine module
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
        OnEachGameObject((renderLayer, gameObject) => gameObject.Drawables.ForEach(drawable =>
        {
            if (renderLayer == RenderLayer.NONE)
            {
                return;
            }
            else if (renderLayer != RenderLayer.UI)
            {
                // convert screen space to world space on non-UI objects?
                Instance.RenderWindow.SetView(Instance.MainView);
            }
            else
            {
                Instance.RenderWindow.SetView(Instance.UiView);
            }
            Instance.RenderWindow.Draw(drawable);
        }));
        Instance.RenderWindow.Display();
    }

    private static List<GameObject> ActiveGameObjects => Instance.GameObjects.Keys.SelectMany(key => Instance.GameObjects[key]).Where(gameObject => gameObject.IsActive).ToList();
    private static List<(RenderLayer renderLayer, GameObject gameObject)> ActiveLayeredGameObjects => 
        Instance.GameObjects.Keys
            .SelectMany(key => 
                Instance.GameObjects[key]
                    .Where(gameObject => gameObject.IsActive)
                    .Select(gameObject => (key, gameObject))
            ).ToList();

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

    private static void OnEachGameObject(Action<RenderLayer, GameObject> doOnEach)
    {
        ActiveLayeredGameObjects.ForEach(renderLayerGoTuple => doOnEach(renderLayerGoTuple.renderLayer, renderLayerGoTuple.gameObject));
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
