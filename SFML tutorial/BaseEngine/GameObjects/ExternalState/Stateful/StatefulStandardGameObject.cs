using SFML.Graphics;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState.Stateful;

public abstract class StatefulStandardGameObject<D, S, STATE> : StatefulGameObject<D, S, STATE>
    where D : Drawable
    where S : StatefulStandardGameObject<D, S, STATE>
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
