using SFML.Graphics;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState.StateDict;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState.StateDict;

public abstract class StandardGameObject<D, S> : GameObject<D, S>
    where D : Drawable
    where S : StandardGameObject<D, S>
{
    public override void Attach()
    {
        OnAttach((S)this);
    }

    public override void Update()
    {
        OnUpdate((S)this);
        foreach (var component in components)
        {
            component.Update();
        }
    }

    public override void Detach()
    {
        OnDetach((S)this);
        foreach (var component in components)
        {
            component.Detach();
        }
    }
}
