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

    private Vector2f moveVelocity = new Vector2f(1, 1);
    private PlayerPaddle? playerPaddle;
    private bool playerIsHoldingBall;

    public Ball()
    {
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
            Position = playerPaddle.Position + new Vector2f(20, playerPaddle.Collider.Bounds.Height / 2);
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
        moveVelocity = new(1, -1); // righward and upward
    }

    private void ReleaseBall()
    {
        playerIsHoldingBall = false;
    }
}
