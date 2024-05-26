using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.PongGame.UI;
namespace SFML_tutorial.Games.PongGame.Entities;
public class ScoreTriggerWall : Positionable
{
    private readonly ScoreText.PlayerId playerId;
    private ScoreText? scoreText;

    public ScoreTriggerWall(ScoreText.PlayerId playerId, FloatRect bounds)
    {
        this.playerId = playerId;
        Collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            Bounds = bounds,
            IsTrigger = true
        };
    }

    public override void Attach()
    {
        scoreText = GameWindow.FindObjectOfType<ScoreText>();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.PositionableGameObject is Ball b)
        {
            scoreText?.AddPointsToPlayer(playerId, 1);
            b.ResetPosision(playerId.Other);
        }
    }
}
