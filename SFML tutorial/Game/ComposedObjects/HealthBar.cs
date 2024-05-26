using SFML.System;
using SFML.Graphics;
using SFML_tutorial.Properties;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.Game.ComposedObjects;

public class HealthBar : UIAnchored
{
    public HealthBar(float maxHp) : this(maxHp, new()) { }
    public HealthBar(float maxHp, Vector2f size)
    {
        curHp = maxHp;
        MaxHp = maxHp;
        outlineBar = new(size)
        {
            FillColor = new(10, 10, 10),
            OutlineColor = Color.Black,
            OutlineThickness = 2,
        };
        contentBar = new(size)
        {
            FillColor = Color.Red,
        };
        hpText = new(maxHp.ToString(), new Font(Resources.Roboto_Black))
        {
            FillColor = Color.Black,
            CharacterSize = 16,
        };
    }

    private readonly RectangleShape outlineBar;
    private readonly RectangleShape contentBar;
    private readonly Text hpText;
    public float MaxHp { get; private set; }
    private bool depleteRight;
    public bool DepleteRight 
    { 
        get => depleteRight;
        set
        {
            depleteRight = value;
            if (depleteRight)
            {
                // make it go the other way
                // Adjust the position so the contentBar will deplete from right to left
                float xDiff = outlineBar.Size.X * curHp / MaxHp;
                contentBar.Position = new Vector2f(outlineBar.Position.X + (outlineBar.Size.X - xDiff), outlineBar.Position.Y);
            }
            else
            {
                // make it deplete left as normal
                contentBar.Position = new Vector2f(outlineBar.Position.X, outlineBar.Position.Y);
            }
        }
    }

    private float curHp;
    public float CurHp
    {
        get => curHp;
        set
        {
            // handle animation of hp bar depleting here
            curHp = value;
            float xDiff = outlineBar.Size.X * curHp / MaxHp;
            contentBar.Size = new Vector2f(xDiff, outlineBar.Size.Y);
            if (DepleteRight)
            {
                contentBar.Position = new Vector2f(outlineBar.Position.X + (outlineBar.Size.X - xDiff), outlineBar.Position.Y);
            }
            hpText.DisplayedString = curHp.ToString("0.00");
        }
    }
    public void TakeDamage(float damage, bool clampedAboveZero = true)
    {
        CurHp = Math.Max(CurHp - damage, clampedAboveZero ? 0 : float.NegativeInfinity);
    }

    private void PositionBars()
    {
        outlineBar.Position = PositionLocally(outlineBar.GetLocalBounds());
        contentBar.Position = DepleteRight
            // Align right edge of contentBar with right edge of outlineBar
            ? new Vector2f(outlineBar.Position.X + (outlineBar.Size.X - contentBar.Size.X), outlineBar.Position.Y)
            // Align left edge of contentBar with left edge of outlineBar
            : outlineBar.Position;
        
        // centers the hpText over the outline bar
        hpText.Position = new Vector2f(
            outlineBar.Position.X + outlineBar.Size.X / 2 - hpText.GetLocalBounds().Width / 2,
            outlineBar.Position.Y + outlineBar.Size.Y / 2 - hpText.CharacterSize / 2
        );
    }

    // ordered from bottom to top
    public override List<Drawable> Drawables => [outlineBar, contentBar, hpText];

    public override required (UIAnchor x, UIAnchor y) Anchors { get; set; } = (UIAnchor.START, UIAnchor.START);
    public override required Vector2f Position { get; set; }

    public override void Update()
    {
        CurHp -= GameWindow.DeltaTime.AsSeconds();
        PositionBars();
    }
}
