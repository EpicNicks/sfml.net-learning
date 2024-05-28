using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

namespace SFML_tutorial.BaseEngine.GameObjects.Composed;

/// <summary>
/// Provides 
///     Lifecycle methods for the state logic which manipulates the encapsulated Drawables
///     Provides a function which returns all associated Drawables
/// </summary>
public class GameObject
{
    public record struct Persistance(bool persistOnSceneTransition, long persistId);
    /// <summary>
    /// Tells the scene if it should pass forward the instance of the GameObject created when the scene transitions to a new scene.
    /// Assigning a unique ID for the engine to know that the GameObject being recreated is the one which has already been created.
    /// Useful for GameObjects which contain globals or otherwise are lifetime objects such as singletons.
    /// </summary>
    public virtual Persistance PersistanceInfo { get; set; } = new Persistance(false, 0L);

    /// <summary>
    /// To be used by the GameWindow to not process inactive GameObjects
    /// </summary>
    public virtual bool IsActive { get; set; } = true;
    /// <summary>
    /// Get all Drawables on the GameObject in inverse order that the drawable will be drawn
    /// </summary>
    public virtual List<Drawable> Drawables { get; set; } = [];

    public virtual Collider2D? Collider { get; set; } = null;

    /// <summary>
    /// Lifecycle method called on GameObject added to the Window.
    /// Can be left unused to empty default implementation.
    /// </summary>
    public virtual void Attach() { }

    /// <summary>
    /// Lifecycle method called on GameObject every frame while it is attached to the Window
    /// Can be left unused to empty default implementation.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Lifecycle method called on GameObject when any of its colliders have collided with another collider of another GameObject
    /// Can be left unused to empty default implementation.
    /// </summary>
    public virtual void OnCollisionEnter2D(Collider2D other) { }
    public virtual void OnCollisionStay2D(Collider2D other) { }
    public virtual void OnCollisionExit2D(Collider2D other) { }
    public virtual void OnTriggerEnter2D(Collider2D other) { }
    public virtual void OnTriggerStay2D(Collider2D other) { }
    public virtual void OnTriggerExit2D(Collider2D other) { }
}
