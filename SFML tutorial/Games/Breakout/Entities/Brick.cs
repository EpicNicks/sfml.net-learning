using SFML.Graphics;

using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.Breakout.UI;

namespace SFML_tutorial.Games.Breakout.Entities;

public class Brick : Positionable
{
    private int hitsTaken = 0;

    private ScoreText? scoreText;

    private readonly int hitsToDestroy;
    private readonly int pointValue;
    private readonly Collider2D collider;
    private readonly RectangleShape rectangleShape;

    public Brick(FloatRect bounds, int hitsToDestroy, int pointValue)
    {
        this.hitsToDestroy = hitsToDestroy;
        this.pointValue = pointValue;
        Position = bounds.Position();
        rectangleShape = new RectangleShape
        {
            FillColor = ColorFromRemainingHits(),
            OutlineColor = ColorFromHits(hitsToDestroy),
            OutlineThickness = 1,
            Position = bounds.Position(),
            Size = bounds.Size(),
        };
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            Bounds = bounds,
        };
    }

    public override void Attach()
    {
        scoreText = GameWindow.FindObjectOfType<ScoreText>();
    }

    public override List<Drawable> Drawables => [rectangleShape];
    public override Collider2D Collider => collider;

    public override void OnCollisionEnter2D(Collider2D other)
    {
        if (other.PositionableGameObject is Ball)
        {
            hitsTaken++;
            rectangleShape.FillColor = ColorFromRemainingHits();
        }
        if (hitsTaken >= hitsToDestroy && scoreText is not null)
        {
            scoreText.Score += pointValue;
            Destroy();
        }
    }

    private Color ColorFromHits(int hits) => hits switch
    {
        3 => Color.Cyan,
        2 => Color.Yellow,
        1 => Color.Red,
        0 => Color.Black, // should be destroyed already
        _ => Color.White // placeholder/error value
    };

    private Color ColorFromRemainingHits() => ColorFromHits(hitsToDestroy - hitsTaken);
}
