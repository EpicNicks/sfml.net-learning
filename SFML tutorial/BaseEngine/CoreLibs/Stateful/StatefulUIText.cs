using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.GameObjects.ExternalState.Stateful;
using SFML_tutorial.BaseEngine.Window.ExternalState;
using SFML_tutorial.Properties;

namespace SFML_tutorial.BaseEngine.CoreLibs.Stateful;

public class StatefulUIText<STATE> : StatefulStandardGameObject<Text, StatefulUIText<STATE>, STATE>
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

    public StatefulUIText(Text text)
    {
        Text = text;
    }
    public StatefulUIText(string str = "") : this(str, new Vector2f(0, 0)) { }
    public StatefulUIText(string str, (float x, float y) position) : this(str, new Vector2f(position.x, position.y)) { }
    public StatefulUIText(string str, Vector2f position)
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
            _ => AnchorToOffsetX(UITextAnchor.START),
        };
        float AnchorToOffsetY(UITextAnchor anchor) => anchor switch
        {
            UITextAnchor.START => 0,
            UITextAnchor.CENTER => (GameWindow.Instance.RenderWindow.Size.Y - Text.GetLocalBounds().Height) / 2f,
            UITextAnchor.END => GameWindow.Instance.RenderWindow.Size.Y - Text.GetLocalBounds().Height,
            _ => AnchorToOffsetY(UITextAnchor.START),
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
