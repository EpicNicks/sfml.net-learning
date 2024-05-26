using SFML.Graphics;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState;
using System.Diagnostics.CodeAnalysis;

namespace SFML_tutorial.BaseEngine.GameObjects.ExternalState.StateDict;

public abstract class GameObject<D, S> : IGameObject<D>
    where D : Drawable
    where S : GameObject<D, S>
{
    protected List<IComponent> components = [];

    protected Dictionary<string, object?> State { get; set; } = [];

    public object? this[string key]
    {
        get => State[key];
        set => State[key] = value;
    }
    public void SetState<U>(string fieldName, U? value)
    {
        State[fieldName] = value;
    }
    public void SetState<U>(string fieldName, Func<U?, object> fieldAction)
    {
        SetState(fieldName, fieldAction.Invoke((U?)State[fieldName]));
    }
    public Action<S> OnAttach { get; set; } = (gameObject) => { };
    public Action<S> OnUpdate { get; set; } = (gameObject) => { };
    public Action<S> OnDetach { get; set; } = (gameObject) => { };

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

    public abstract D Drawable { get; }
    public abstract void Attach();
    public abstract void Update();
    public abstract void Detach();
}
