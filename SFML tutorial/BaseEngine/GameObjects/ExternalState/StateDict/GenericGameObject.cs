using SFML.Graphics;
using SFML_tutorial.BaseEngine.Window;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState.StateDict;

/// <summary>
/// Empty GameObject wrapper for drawables
/// </summary>
/// <param name="drawable"></param>
public class GenericGameObject<D>(D drawable) : StandardGameObject<D, GenericGameObject<D>>
    where D : Drawable
{
    public override D Drawable => drawable;
}
