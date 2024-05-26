using SFML.Graphics;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState;

/// <summary>
/// The specialized Component interface to contain a Drawable and other Components
/// </summary>
/// <typeparam name="D">The Drawable this GameObject Contains</typeparam>
public interface IGameObject<out D> : IComponent
    where D : Drawable
{
    // Transform is attached to Drawable so this is analogous to the base component of a Unity GameObject
    D Drawable { get; }
}
