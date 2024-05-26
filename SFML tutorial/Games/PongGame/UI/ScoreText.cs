using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.Properties;

namespace SFML_tutorial.Games.PongGame.UI;

public class ScoreText : UIAnchored
{
    public abstract record PlayerId
    {
        private PlayerId() { }
        public PlayerId Other => this switch
        {
            One => two,
            Two => one,
            _ => throw new InvalidOperationException("playerId was not one or two")
        };
        public sealed record One : PlayerId { }
        public sealed record Two : PlayerId { }

        public static One one => new One();
        public static Two two => new Two();
    }

    private readonly Text scoreText;

    private (long player1, long player2) score = (0, 0);
    public (long player1, long player2) Score
    {
        get => score;
        set
        {
            score = value;
            scoreText.DisplayedString = ScoreToDisplayedText;
        }
    }

    private string ScoreToDisplayedText => $"Score: {Score.player1}-{Score.player2}";

    public ScoreText()
    {
        scoreText = new Text
        {
            Font = new Font(Resources.Roboto_Black),
            CharacterSize = 28,
            FillColor = Color.White,
            DisplayedString = ScoreToDisplayedText,
        };
    }

    public override List<Drawable> Drawables => [scoreText];

    public override void Attach()
    {
        scoreText.Position = PositionLocally(scoreText.GetLocalBounds());
    }


    public void AddPointsToPlayer(PlayerId playerId, long scoreToAdd)
    {
        Score = playerId switch
        {
            PlayerId.One => (Score.player1 + scoreToAdd, Score.player2),
            PlayerId.Two => (Score.player1, Score.player2 + scoreToAdd),
            _ => throw new Exception("Closed set was not closed. Contact the CEO of Mathematics.")
        };
    }

    public void UpdateScore(PlayerId playerId, long score)
    {
        Score = playerId switch
        {
            PlayerId.One => (score, Score.player2),
            PlayerId.Two => (Score.player1, score),
            _ => throw new Exception("Closed set was not closed. Contact the CEO of Mathematics.")
        };
    }

    public void ClearScores()
    {
        Score = (0, 0);
    }

}
