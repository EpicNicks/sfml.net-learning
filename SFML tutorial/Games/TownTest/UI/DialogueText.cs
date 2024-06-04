using SFML.Graphics;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.Properties;

namespace SFML_tutorial.Games.TownTest.UI;
public class DialogueText : UIAnchored
{
    private string displayString = "";
    private readonly Text text;
    private readonly Font font;

    public DialogueText()
    {
        font = new Font(Resources.Roboto_Black);
        text = new Text
        {
            Font = font,
            DisplayedString = DisplayString,
            CharacterSize = 24,
        };
    }

    public string DisplayString
    {
        get => displayString;
        set 
        {
            // just set value for now
            // can try to animate the text in update or something later
            displayString = value;
            text.DisplayedString = displayString;
        }
    }

    public override (UIAnchor x, UIAnchor y) Anchors => (UIAnchor.CENTER, UIAnchor.END);

    public override List<Drawable> Drawables => [text];

    public override void Update()
    {
        text.Position = PositionLocally(text.GetLocalBounds());
    }
}
