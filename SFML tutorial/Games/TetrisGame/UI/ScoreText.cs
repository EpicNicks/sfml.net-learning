using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.Properties;

namespace SFML_tutorial.Games.TetrisGame.UI;
public class ScoreText : UIAnchoredText
{
    private int score;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            text.DisplayedString = $"Score: {Score}";
        }
    }

    public ScoreText() : base("Score: 0", new Font(Resources.Roboto_Black), Color.White, 24) { }

    public override (UIAnchor x, UIAnchor y) Anchors => (UIAnchor.CENTER, UIAnchor.START);
}
