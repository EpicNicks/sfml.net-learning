using SFML.Graphics;

namespace SFML_tutorial.BaseEngine.CoreLibs.Composed;
/// <summary>
/// A common implementation for a UIAnchored of a single text element
/// </summary>
public class UIAnchoredText : UIAnchored
{
    protected readonly Text text;

    public UIAnchoredText(string initialText, Font font, Color fontColor, uint fontSize)
    {
        text = new Text
        {
            Font = font,
            CharacterSize = fontSize,
            FillColor = fontColor,
            DisplayedString = initialText,
        };
    }

    public override List<Drawable> Drawables => [text];

    public override (UIAnchor x, UIAnchor y) Anchors 
    { 
        get => base.Anchors; set
        {
            base.Anchors = value;
            // required because properties set after the constructor is run
            text.Position = PositionLocally(text.GetLocalBounds());
        }
    }

    public override void Update()
    {
        text.Position = PositionLocally(text.GetLocalBounds());
    }
}
