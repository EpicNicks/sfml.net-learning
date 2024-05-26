using SFML_tutorial.BaseEngine.GameObjects.ExternalState;

namespace SFML_tutorial.Game.Components;

public class HPComponent : IComponent
{
    public int MaxHp { get; set; }
    public int CurHp { get; set; }

    public HPComponent() { }
    public HPComponent(int maxHp)
    {
        MaxHp = CurHp = maxHp;
    }
    public void Attach()
    {

    }

    public void Update()
    {
        if (CurHp < 0)
        {
            // death logic
        }
    }

    public void Detach()
    {

    }
}
