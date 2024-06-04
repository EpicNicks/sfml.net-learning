using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.BaseEngine.Scheduling.Coroutines;
/// <summary>
/// The base interface for all Coroutines's IEnumerator's current yield.
/// Provides bool Wait() to tell the Coroutine Scheduler how it should wait based on the pattern-matched instance returned.s
/// </summary>
public interface ICoroutineWait
{
    /// <summary>
    /// To be called once per frame in Update
    /// </summary>
    /// <returns>true if the caller should wait, false if the caller should continue</returns>
    bool Wait();
}

/// <summary>
/// Used in IEnumerators wrapped by Coroutines to tell it to delay execution to the next frame.
/// Equivalent to a null yield return but with more explicit intent.
/// </summary>
public class WaitForNextFrame : ICoroutineWait
{
    private bool waitCalled = false;

    public bool Wait()
    {
        if (!waitCalled)
        {
            waitCalled = true;
            return true;
        }
        return false;
    }
}

/// <summary>
/// Used in IEnumerators wrapped by Coroutines to tell it to delay execution until waitSeconds has elapsed.
/// </summary>
/// <param name="waitSeconds">The number of seconds to wait</param>
public class WaitForSeconds(float waitSeconds) : ICoroutineWait
{
    private readonly float waitSeconds = waitSeconds;
    private float elapsedSeconds = 0f;

    public bool Wait()
    {
        elapsedSeconds += GameWindow.DeltaTime.AsSeconds();
        if (elapsedSeconds < waitSeconds)
        {
            return true;
        }
        return false;
    }
}
