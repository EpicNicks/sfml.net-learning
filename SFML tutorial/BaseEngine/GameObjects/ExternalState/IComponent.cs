using System;
namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState;

/// <summary>
/// The base interface for reusable components
/// </summary>
public interface IComponent
{
    /// <summary>
    /// Run when the component is added to the window
    /// </summary>
    void Attach();
    /// <summary>
    /// Run on every update call of the GameObject this is attached to
    /// </summary>
    void Update();
    /// <summary>
    /// Run when the GameObject is detached
    /// </summary>
    void Detach();
}
