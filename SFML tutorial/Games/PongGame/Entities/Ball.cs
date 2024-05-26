using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Math;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.PongGame.UI;

namespace SFML_tutorial.Games.PongGame.Entities;
public class Ball : Moveable
{
    private readonly CircleShape circle;
    private readonly Collider2D collider;
    private Moveable? player1, player2;
    // start diagonally up and to the right
    private Vector2f moveVelocity = new Vector2f(-1, 1);
    private Moveable? playerHoldingBall;
    public Moveable? PlayerHoldingBall => playerHoldingBall;

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
    public void ReleaseBall()
    {
        playerHoldingBall = null;
    }
    public void ResetPosision(ScoreText.PlayerId playerId)
    {
        SetInitialPosition(playerId);
        // reverse the X velocity to make the ball move toward the player who just scored
        // moveVelocity.X *= -1;
    }

    public override void Attach()
    {
        FindAndSetPaddles();
        SetInitialPosition(ScoreText.PlayerId.one);
        GameWindow.Instance.RenderWindow.KeyPressed += (_, e) =>
        {
            if (e.Code == Keyboard.Key.Space)
            {
                ReleaseBall();
            }
        };
    }

    public override void Update()
    {
        if (playerHoldingBall != null)
        {
            if (playerHoldingBall == player1)
            {
                Position = playerHoldingBall.Position + new Vector2f(20, playerHoldingBall.Collider?.Bounds.Height / 2 ?? 0);
            }
            else
            {
                Position = playerHoldingBall.Position + new Vector2f(-20, playerHoldingBall.Collider?.Bounds.Height / 2 ?? 0);
            }
        }
        else
        {
            Move(moveVelocity);
        }
    }

    public override void OnCollisionEnter2D(Collider2D other)
    {
        Collisions.SideHit sideHit = Collisions.GetCollisionSide(other, collider);
        // determine collision normal, change moveVelocity
        // sides are pretty much absolute directions as they are

        moveVelocity = sideHit switch
        {
            Collisions.SideHit.TOP => new Vector2f(moveVelocity.X, -moveVelocity.Y), // must have been moving downward
            Collisions.SideHit.BOTTOM => new Vector2f(moveVelocity.X, -moveVelocity.Y), // must have been moving upward
            Collisions.SideHit.LEFT => new Vector2f(-moveVelocity.X, moveVelocity.Y), // must have been moving right
            Collisions.SideHit.RIGHT => new Vector2f(-moveVelocity.X, moveVelocity.Y), // must have been moving left
            _ => moveVelocity // also HOW
        };
    }

    public override List<Drawable> Drawables => [circle];
    public override Collider2D Collider => collider;

    private void SetInitialPosition(ScoreText.PlayerId playerId)
    {
        (playerHoldingBall, moveVelocity) = playerId switch
        {
            ScoreText.PlayerId.One => (player1, new Vector2f(1, -1)),
            ScoreText.PlayerId.Two => (player2, new Vector2f(-1, 1)),
            _ => (player1, new Vector2f(1, -1)), // HOW
        };

        // center of window (obsolete)
        //Vector2u gameWindowSize = GameWindow.Instance.RenderWindow.Size;
        //Position = new Vector2f(gameWindowSize.X / 2 - circle.Radius, gameWindowSize.Y / 2 - circle.Radius);
    }

    private void FindAndSetPaddles()
    {
        List<PlayerPaddle> playerPaddles = GameWindow.FindObjectsOfType<PlayerPaddle>();
        List<AIPaddle> aiPaddles = GameWindow.FindObjectsOfType<AIPaddle>();
        foreach (var paddle in playerPaddles)
        {
            if (paddle.IsLeftSidePlayer)
            {
                player1 = paddle;
            }
            else
            {
                player2 = paddle;
            }
        }
        foreach (var paddle in aiPaddles)
        {
            if (paddle.IsLeftSidePlayer)
            {
                player1 = paddle;
            }
            else
            {
                player2 = paddle;
            }
        }
    }
}
