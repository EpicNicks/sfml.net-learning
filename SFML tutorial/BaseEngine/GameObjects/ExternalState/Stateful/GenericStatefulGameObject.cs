using SFML.Graphics;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState.Stateful;

public class GenericStatefulGameObject<D, STATE>(D drawable) : StatefulStandardGameObject<D, GenericStatefulGameObject<D, STATE>, STATE>
    where D : Drawable
{
    public override D Drawable => drawable;
}
