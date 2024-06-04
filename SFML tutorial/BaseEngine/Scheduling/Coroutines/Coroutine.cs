using System.Collections;
namespace SFML_tutorial.BaseEngine.Scheduling.Coroutines;
public class Coroutine
{
    private object? currentYield = null;
    private IEnumerator Routine { get; set; }
    public bool IsComplete { get; private set; } = false;

    public Coroutine(IEnumerator routine)
    {
        Routine = routine;
    }

    /// <summary>
    /// Advances the inner IEnumerator to the next yield
    /// </summary>
    /// <returns>Whether or not the coroutine has code left to execute</returns>
    public bool MoveNext()
    {
        #region Check Result Of Last Call If Exists
        if (IsComplete)
        {
            return false;
        }
        if (currentYield is ICoroutineWait coroutineWait && coroutineWait.Wait())
        {
            // Continue waiting
            return true;
        }
        #endregion
        #region Next Iterator Call
        if (Routine.MoveNext())
        {
            currentYield = Routine.Current;
            return true;
        }
        else
        {
            // Coroutine has completed
            IsComplete = true;
            return false;
        }
        #endregion
    }

}
