using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.Breakout.UI;

namespace SFML_tutorial.Games.Breakout.Entities;
/// <summary>
/// Another persistent GameObject in Scene
/// </summary>
public class FailureTrigger : Positionable
{
    private Collider2D collider;
    private TriesText? triesText;
    private readonly RectangleShape debugRect;
    private readonly bool debug;

    public FailureTrigger(FloatRect bounds, bool debug = false)
    {
        this.debug = debug;
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            Bounds = bounds,
            IsTrigger = true,
        };
        debugRect = new RectangleShape
        {
            FillColor = Color.Black,
            OutlineColor = Color.Red,
            OutlineThickness = 1,
            Position = bounds.Position(),
            Size = bounds.Size()
        };
    }
    public override Collider2D Collider => collider;
    public override List<Drawable> Drawables => debug ? [debugRect] : [];

    public override void Attach()
    {
        triesText = GameWindow.FindObjectOfType<TriesText>();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (triesText is not null)
        {
            triesText.CurrentTriesAmount--;
        }
    }
}
