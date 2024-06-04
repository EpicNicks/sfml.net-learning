using SFML_tutorial.BaseEngine.GameObjects.Composed;
using System.Collections;

namespace SFML_tutorial.BaseEngine.Scheduling.Coroutines;
/// <summary>
/// Simple Dictionary-based scheduler.
/// You probably don't have enough concurrently scheduled Coroutines to warrant a data structure with more overhead but better big O time complexity for large N.
/// </summary>
public class CoroutineScheduler
{
    private Dictionary<GameObject, List<Coroutine>> Coroutines { get; set; } = [];

    /// <summary>
    /// Called by the GameWindow/Application on each frame
    /// </summary>
    public void Update()
    {
        List<(GameObject, Coroutine)> coroutinesToRemove = [];
        var gameObjects = Coroutines.Keys.ToList();

        foreach (GameObject gameObject in gameObjects)
        {
            foreach (Coroutine coroutine in Coroutines[gameObject].ToList())
            {
                bool isComplete = AdvanceCoroutine(coroutine);
                if (isComplete)
                {
                    coroutinesToRemove.Add((gameObject, coroutine));
                }
            }
        }
        CleanupCompleted(coroutinesToRemove);
    }

    public void StartCoroutine(GameObject gameObject, IEnumerator routine)
    {
        Coroutine coroutine = new Coroutine(routine);
        if (Coroutines.TryGetValue(gameObject, out List<Coroutine>? value))
        {
            value.Add(coroutine);
        }
        else
        {
            Coroutines[gameObject] = [coroutine];
        }
    }

    public bool RemoveGameObject(GameObject gameObject)
    {
        return Coroutines.Remove(gameObject);
    }

    /// <summary>
    /// Advances the passed Coroutine
    /// </summary>
    /// <param name="coroutine">The Coroutine to advance</param>
    /// <returns>whether or not the coroutine has completed execution</returns>
    private static bool AdvanceCoroutine(Coroutine coroutine)
    {
        return !coroutine.MoveNext();
    }

    private void CleanupCompleted(List<(GameObject, Coroutine)> coroutinesToRemove)
    {
        foreach ((GameObject gameObject, Coroutine coroutine) in coroutinesToRemove)
        {
            Coroutines[gameObject].Remove(coroutine);
            if (Coroutines[gameObject].Count == 0)
            {
                RemoveGameObject(gameObject);
            }
        }
    }
}
