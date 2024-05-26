using SFML.Graphics;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState;
using System.Diagnostics.CodeAnalysis;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState.Stateful;

public abstract class StatefulGameObject<D, S, STATE> : IGameObject<D>
    where D : Drawable
    where S : StatefulGameObject<D, S, STATE>
{
    public required STATE State { get; set; }
    protected List<IComponent> components = [];

    private void AddComponentGood(IComponent component)
    {
        components.Add(component);
        component.Attach();
    }
    public C? AddComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] C>() where C : IComponent
    {
        if (!components.Any((c) => c.GetType() == typeof(C)))
        {
            var component = Activator.CreateInstance<C>();
            AddComponentGood(component);
            return component;
        }
        return default;
    }
    public void AddComponent<C>(C component) where C : IComponent
    {
        if (!components.Any((c) => c.GetType() == typeof(C)))
        {
            AddComponentGood(component);
        }
    }
    public bool RemoveComponent<C>(C component) where C : IComponent
    {
        return components.Remove(component);
    }
    public bool RemoveComponent<C>() where C : IComponent
    {
        var component = components.Find((c) => c.GetType() == typeof(C));
        return component == null || components.Remove(component);
    }

    public C? GetComponent<C>() where C : IComponent
    {
        return (C?)components.Find((c) => c.GetType() == typeof(C));
    }

    public Action<S> OnAttach { get; set; } = (gameObject) => { };
    public Action<S> OnUpdate { get; set; } = (gameObject) => { };
    public Action<S> OnDetach { get; set; } = (gameObject) => { };

    public abstract D Drawable { get; }
    public abstract void Attach();
    public abstract void Update();
    public abstract void Detach();
}
