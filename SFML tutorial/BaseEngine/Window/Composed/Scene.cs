using SFML_tutorial.BaseEngine.GameObjects.Composed;

namespace SFML_tutorial.BaseEngine.Window.Composed;

/// <summary>
/// A GameObject container that handles creation and cleanup of GameObjects into the GameWindow
/// </summary>
public class Scene
{
    private readonly string name;
    public string Name => name;
    private readonly Action<Action<RenderLayer, GameObject>> initFunction;
    private SortedDictionary<RenderLayer, List<GameObject>> gameObjects = new(Comparer<RenderLayer>.Create((l, r) => l - r));
    public SortedDictionary<RenderLayer, List<GameObject>> GameObjects => gameObjects;

    public Scene(string name, Action<Action<RenderLayer, GameObject>> initFunction)
    {
        this.name = name;
        this.initFunction = initFunction;
    }

    /// <summary>
    /// Initializes the scene with the initFunction provided, giving it the "Add" method.
    /// This allows the scene to be recreated in the state the objects were originally constructed in.
    /// example usage:
    /// new Scene("'", (add) =>
    /// {
    ///     add(RenderLayerEnumValue, new SomeGameObjectDerivedClass);
    /// });
    /// </summary>
    public void Init()
    {
        initFunction(Add);
    }

    public bool Contains(RenderLayer renderLayer, GameObject gameObject)
    {
        return GameObjects.ContainsKey(renderLayer) && GameObjects[renderLayer].Contains(gameObject);
    }
    public bool Contains(GameObject gameObject)
        => GameObjects.Keys.Any(renderLayer => Contains(renderLayer, gameObject));

    public void Add(RenderLayer renderLayer, GameObject gameObject)
    {
        if (Contains(gameObject))
        {
            throw new InvalidOperationException("GameObject was already added to window!");
        }
        if (GameObjects.TryGetValue(renderLayer, out List<GameObject>? value))
        {
            value.Add(gameObject);
        }
        else
        {
            GameObjects[renderLayer] = [gameObject];
        }
        GameWindow.Instance.AttachQueue.Enqueue(gameObject);
    }
    public void Add(List<(RenderLayer renderLayer, GameObject gameObject)> layeredGameObjects)
    {
        foreach (var (renderLayer, gameObject) in layeredGameObjects)
        {
            Add(renderLayer, gameObject);
        }
    }

    public void RegisterToGameWindow()
    {
        // add to ordered scene list (a List<T> should suffice)
    }

    public static void LoadNext()
    {

    }

    public static void LoadScene(string name)
    {

    }
}
