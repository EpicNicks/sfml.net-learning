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

    public required bool IsLeftSidePlayer { get; set; }

    public AIPaddle(Vector2f size)
    {
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            IsTrigger = false,
            Bounds = new FloatRect(Position, size)
        };
        rectangleShape = new RectangleShape
        {
            FillColor = Color.White,
            Size = size,
            Position = Position,
        };
    }

    public override List<Drawable> Drawables => [rectangleShape];
    public override Collider2D Collider => collider;

    public override void Attach()
    {
        Position = new Vector2f(Position.X, GameWindow.Size.Y / 2f);
        ballToWatch = GameWindow.FindObjectOfType<Ball>();
    }

    public override void Update()
    {
        HandleResize();
        FollowBallAndIdleStrategy();
        //FollowBallMoveStragety();
    }

    /// <summary>
    /// Moves to center screen while ball is not on its half of the screen
    /// </summary>
    private void FollowBallAndIdleStrategy()
    {
        float centerOfScreen = GameWindow.Instance.RenderWindow.Size.Y / 2;
        float centerOfPaddlePosY = collider.Bounds.Top + collider.Bounds.Height / 2f;
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
        if 
        (
            !IsLeftSidePlayer && ballToWatch?.Position.X <= GameWindow.Instance.RenderWindow.Size.X / 2f
            ||
            IsLeftSidePlayer && ballToWatch?.Position.X > GameWindow.Instance.RenderWindow.Size.X / 2f
        )
        {
            targetPos = centerOfScreen;
        }
        float simulatedYInput = 0f;
        float threshold = 1.0f;  // Change this value based on what works best for your scenario
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
        simulatedYInput *= GameWindow.Size.Y / 800; // scaled compared to the original window size of x=1200 y=800
        Move(new Vector2f(0, simulatedYInput));
        Position = new Vector2f(Position.X, Math.Clamp(Position.Y, 0, GameWindow.Instance.RenderWindow.Size.Y - Collider.Bounds.Height));
    }

    /// <summary>
    /// Executes strategy to follow the ball on the Y axis 1:1 using the provided moveSpeed
    /// </summary>
    private void FollowBallMoveStragety()
    {
        float centerOfScreen = GameWindow.Instance.RenderWindow.Size.Y / 2 + Collider.Bounds.Height;
        float centerOfPaddlePosY = collider.Bounds.Top + collider.Bounds.Height / 2f;
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
    private void HandleResize()
    {
        if (IsLeftSidePlayer)
        {
            Position = new Vector2f(200, Position.Y);
        }
        else
        {
            Position = new Vector2f(GameWindow.Size.X - 200, Position.Y);
        }
    }
}
