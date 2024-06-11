using System.Collections;

using SFML.Graphics;

using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Scheduling.Coroutines;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Properties;

namespace SFML_tutorial.Games.Breakout.UI;
public class ScoreText : UIAnchored
{
    private readonly Text scoreText;
    private int score;
    public int Score
    {
        get => score;
        set
        {
            if (value > score)
            {
                StopAllCoroutines();
                StartCoroutine(ScoreColorFlashGood());
            }
            if (value < score)
            {
                StopAllCoroutines();
                StartCoroutine(ScoreColorFlashBad());
            }
            score = value;
        }
    }

    public ScoreText()
    {
        scoreText = new Text
        {
            Font = new(Resources.Roboto_Black),
            CharacterSize = 24,
            FillColor = Color.White,
        };
    }

    public override void Update()
    {
        scoreText.DisplayedString = (GameWindow.LoadedSceneName == "Game Over" || GameWindow.LoadedSceneName == "Victory Screen") 
            ? $"Final Score: {Score}" : Score.ToString();
        scoreText.Position = PositionLocally(scoreText.GetLocalBounds());
    }

    public override (UIAnchor x, UIAnchor y) Anchors => (UIAnchor.CENTER, UIAnchor.START);

    public override List<Drawable> Drawables => [scoreText];

    private IEnumerator ScoreColorFlashGood()
    {
        scoreText.FillColor = Color.Green;
        yield return new WaitForSeconds(0.1f);
        scoreText.FillColor = Color.White;
        yield return new WaitForSeconds(0.1f);
        scoreText.FillColor = Color.Green;
        yield return new WaitForSeconds(0.1f);
        scoreText.FillColor = Color.White;
    }

    private IEnumerator ScoreColorFlashBad()
    {
        scoreText.FillColor = Color.Red;
        yield return new WaitForSeconds(0.1f);
        scoreText.FillColor = Color.White;
        yield return new WaitForSeconds(0.1f);
        scoreText.FillColor = Color.Red;
        yield return new WaitForSeconds(0.1f);
        scoreText.FillColor = Color.White;
    }

}
