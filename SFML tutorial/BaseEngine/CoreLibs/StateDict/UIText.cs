using SFML_tutorial.Properties;

using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState.StateDict;
using SFML_tutorial.BaseEngine.Window.ExternalState;

namespace SFML_tutorial.BaseEngine.CoreLibs.StateDict;

public class UIText : StandardGameObject<Text, UIText>
{
    public Text Text { get; private set; }
    public override Text Drawable => Text;

    public enum UITextAnchor
    {
        START,
        CENTER,
        END,
    }

    public Vector2f LocalPosition { get; set; }
    public (UITextAnchor x, UITextAnchor y) UITextAnchors { get; set; } = (UITextAnchor.START, UITextAnchor.START);

    public UIText(Text text)
    {
        Text = text;
    }
    public UIText(string str = "") : this(str, new Vector2f(0, 0)) { }
    public UIText(string str, (float x, float y) position) : this(str, new Vector2f(position.x, position.y)) { }
    public UIText(string str, Vector2f position)
    {
        Text = new Text(str, new Font(Resources.Roboto_Black));
        LocalPosition = position;
    }

    private void PositionText()
    {
        float AnchorToOffsetX(UITextAnchor anchor) => anchor switch
        {
            UITextAnchor.START => 0,
            UITextAnchor.CENTER => (GameWindow.Instance.RenderWindow.Size.X - Text.GetLocalBounds().Width) / 2f,
            UITextAnchor.END => GameWindow.Instance.RenderWindow.Size.X - Text.GetLocalBounds().Width,
            _ => 0,
        };
        float AnchorToOffsetY(UITextAnchor anchor) => anchor switch
        {
            UITextAnchor.START => 0,
            UITextAnchor.CENTER => (GameWindow.Instance.RenderWindow.Size.Y - Text.GetLocalBounds().Height) / 2f,
            UITextAnchor.END => GameWindow.Instance.RenderWindow.Size.Y - Text.GetLocalBounds().Height,
            _ => 0,
        };

        Text.Position = LocalPosition + new Vector2f(AnchorToOffsetX(UITextAnchors.x), AnchorToOffsetY(UITextAnchors.y));
    }

    public override void Attach()
    {
        base.Attach();
    }

    public override void Update()
    {
        PositionText();
        base.Update();
    }
}
