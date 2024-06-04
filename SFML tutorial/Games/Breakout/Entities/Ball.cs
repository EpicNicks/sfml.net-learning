using SFML.Graphics;
using SFML.System;
using SFML.Window;

using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.Games.Breakout.Entities;
public class Ball : Moveable
{
    private readonly CircleShape circle;
    private readonly Collider2D collider;
    // righward and upward
    private readonly Vector2f INITIAL_VELOCITY = new Vector2f(0.5f, -1);

    private Vector2f moveVelocity;
    private PlayerPaddle? playerPaddle;
    private bool playerIsHoldingBall;

    public Ball()
    {
        moveVelocity = INITIAL_VELOCITY;
        float circleRadius = 5f;
        circle = new CircleShape
        {
            Radius = circleRadius,
            FillColor = Color.White,
        };
        collider = new Collider2D
        {
            Bounds = new FloatRect(0, 0, circleRadius * 2, circleRadius * 2),
            IsStatic = false,
            PositionableGameObject = this,
            IsTrigger = false
        };
    }
    public override List<Drawable> Drawables => [circle];
    public override Collider2D Collider => collider;

    public override void Attach()
    {
        playerPaddle = GameWindow.FindObjectOfType<PlayerPaddle>();
        SetInitialPosition();
        GameWindow.Instance.RenderWindow.KeyPressed += OnKeyPressed;
    }

    public override void OnDestroy()
    {
        GameWindow.Instance.RenderWindow.KeyPressed -= OnKeyPressed;
    }

    public override void Update()
    {
        HandlePosition();
        HandleOOB();
    }

    public override void OnCollisionEnter2D(Collider2D other)
    {
        Collisions.SideHit sideHit = Collisions.GetCollisionSide(other, collider);
        moveVelocity = sideHit switch
        {
            Collisions.SideHit.TOP => new Vector2f(moveVelocity.X, -moveVelocity.Y), // must have been moving downward
            Collisions.SideHit.BOTTOM => new Vector2f(moveVelocity.X, -moveVelocity.Y), // must have been moving upward
            Collisions.SideHit.LEFT => new Vector2f(-moveVelocity.X, moveVelocity.Y), // must have been moving right
            Collisions.SideHit.RIGHT => new Vector2f(-moveVelocity.X, moveVelocity.Y), // must have been moving left
            _ => moveVelocity // also HOW
        };
    }

    private void OnKeyPressed(object? _, KeyEventArgs key)
    {
        if (key.Code == Keyboard.Key.Space)
        {
            ReleaseBall();
        }
    }

    private void HandlePosition()
    {
        if (playerIsHoldingBall && playerPaddle is not null)
        {
            Position = playerPaddle.Position + new Vector2f(playerPaddle.Collider.Bounds.Width / 2, -20);
        }
        else
        {
            Move(moveVelocity);
        }
    }

    private void HandleOOB()
    {
        Vector2f gameWindowSize = GameWindow.Instance.MainView.Size;
        if (Position.X < -10 || Position.X > gameWindowSize.X + 10 || Position.Y < -10 || Position.Y > gameWindowSize.Y + 10)
        {
            SetInitialPosition();
        }
    }

    private void SetInitialPosition()
    {
        playerIsHoldingBall = true;
        moveVelocity = INITIAL_VELOCITY;
    }

    private void ReleaseBall()
    {
        playerIsHoldingBall = false;
    }
}
