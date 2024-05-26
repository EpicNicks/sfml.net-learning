using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.Games.PongGame.Entities;
public class AIPaddle : Moveable
{
    private Ball? ballToWatch;
    private readonly Collider2D collider;
    private readonly RectangleShape rectangleShape;
    private float curBallHoldingSeconds = 0.0f;
    private readonly float BALL_HOLDING_SECONDS = 3.0f;

    public AIPaddle(FloatRect bounds)
    {
        Position = new Vector2f(bounds.Left, bounds.Top);
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            IsTrigger = false,
            Bounds = bounds
        };
        rectangleShape = new RectangleShape
        {
            FillColor = Color.White,
            Size = new Vector2f(bounds.Width, bounds.Height),
            Position = Position,
        };
    }

    public override List<Drawable> Drawables => [rectangleShape];
    public override Collider2D Collider => collider;

    public override void Attach()
    {
        ballToWatch = GameWindow.FindObjectOfType<Ball>();
    }

    public override void Update()
    {
        float centerOfScreen = GameWindow.Instance.RenderWindow.Size.Y / 2 - Collider.Bounds.Height;
        float centerOfPaddlePosY = collider.Bounds.Top - collider.Bounds.Height / 2f;
        float targetPos = ballToWatch?.Position.Y ?? centerOfScreen;
        if (ballToWatch?.PlayerHoldingBall == this)
        {
            if (curBallHoldingSeconds < BALL_HOLDING_SECONDS)
            {
                // move toward center screen on y
                curBallHoldingSeconds += GameWindow.DeltaTime.AsSeconds();
                targetPos = centerOfScreen;
            }
            else
            {
                ballToWatch.ReleaseBall();
                curBallHoldingSeconds = 0.0f;
            }
        }
        float simulatedYInput = 0f;
        float threshold = 5.0f;  // Change this value based on what works best for your scenario
        if (Math.Abs(centerOfPaddlePosY - targetPos) > threshold)
        {
            if (centerOfPaddlePosY < targetPos)
            {
                simulatedYInput = 1f;
            }
            else if (centerOfPaddlePosY > targetPos)
            {
                simulatedYInput = -1f;
            }
        }
        Move(new Vector2f(0, simulatedYInput));
        Position = new Vector2f(Position.X, Math.Clamp(Position.Y, 0, GameWindow.Instance.RenderWindow.Size.Y - Collider.Bounds.Height));
    }
}
