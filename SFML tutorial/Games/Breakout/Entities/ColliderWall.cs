using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;

namespace SFML_tutorial.Games.Breakout.Entities;

public class ColliderWall : Positionable
{
    private readonly bool debug;
    private readonly Collider2D collider;
    private readonly RectangleShape debugRect;

    public ColliderWall(FloatRect bounds, bool debug = false)
    {
        this.debug = debug;
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            Bounds = bounds,
            IsTrigger = false
        };
        debugRect = new RectangleShape
        {
            FillColor = Color.Black,
            OutlineColor = Color.Green,
            OutlineThickness = 1,
            Size = bounds.Size(),
            Position = bounds.Position()
        };
    }

    public override Collider2D Collider => collider;
    public override List<Drawable> Drawables => debug ? [debugRect] : [];
}
