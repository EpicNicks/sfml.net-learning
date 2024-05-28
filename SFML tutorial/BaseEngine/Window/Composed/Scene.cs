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
    /// Adds all objects passed to the callback function to the gameObjects List.
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

    public List<T> FindObjectsOfType<T>()
    {
        List<T> result = [];
        foreach (var gameObject in GameObjects.Keys.SelectMany(key => GameObjects[key]))
        {
            if (gameObject is T t)
            {
                result.Add(t);
            }
        }
        return result;
    }

    public T? FindObjectOfType<T>(RenderLayer renderLayer)
    {
        foreach (var gameObject in GameObjects[renderLayer])
        {
            if (gameObject is T t)
            {
                return t;
            }
        }
        return default;
    }

    public T? FindObjectOfType<T>()
    {
        foreach (var gameObject in GameObjects.Keys.SelectMany(key => GameObjects[key]))
        {
            if (gameObject is T t)
            {
                return t;
            }
        }
        return default;
    }

    public bool TryRemove(RenderLayer renderLayer, GameObject gameObject)
        => GameObjects.ContainsKey(renderLayer) && GameObjects[renderLayer].Remove(gameObject);
    public bool TryRemove(GameObject gameObject)
    {
        foreach (RenderLayer key in GameObjects.Keys)
        {
            if (TryRemove(key, gameObject))
            {
                return true;
            }
        }
        return false;
    }

    public List<GameObject> Unload()
    {
        List<GameObject> passForward = [];
        foreach (GameObject gameObject in GameObjects.Keys.SelectMany(key => GameObjects[key]))
        {
            if (gameObject.PersistOnSceneTransition)
            {
                passForward.Add(gameObject);
            }
            else
            {
                gameObject.IsActive = false;
            }
        }
        gameObjects.Clear();
        return passForward;
    }
}
