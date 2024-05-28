using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.PongGame.UI;
namespace SFML_tutorial.Games.PongGame.Entities;
public class ScoreTriggerWall : Positionable
{
    private readonly Collider2D collider;
    private readonly bool isLeftTrigger;
    private readonly ScoreText.PlayerId playerId;
    private ScoreText? scoreText;

    public ScoreTriggerWall(bool isLeftTrigger)
    {
        this.isLeftTrigger = isLeftTrigger;
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            IsTrigger = true
        };
        playerId = isLeftTrigger ? ScoreText.PlayerId.two : ScoreText.PlayerId.one;
    }

    public override Collider2D Collider => collider;

    public override void Attach()
    {
        scoreText = GameWindow.FindObjectOfType<ScoreText>();
    }

    public override void Update()
    {
        // TODO: handle resize/reposition whether IsLeftTrigger is true or not
        // also remove bounds from the constructor,
        //  size and bounds are completely dictated by the screen dimensions
        Collider.Bounds = isLeftTrigger 
            ? new FloatRect(0, 0, 10, GameWindow.Size.Y) 
            : new FloatRect(GameWindow.Size.X - 10, 0, 10, GameWindow.Size.Y);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.PositionableGameObject is Ball ball)
        {
            scoreText?.AddPointsToPlayer(playerId, 1);
            ball.ResetPosision(playerId.Other);
        }
    }
}
