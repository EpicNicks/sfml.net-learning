using SFML.System;
using SFML.Graphics;
using SFML.Window;
using static SFML.Window.Keyboard;

using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;
using SFML_tutorial.BaseEngine.CoreLibs.SFMLExtensions;
using System.Collections;
using SFML_tutorial.BaseEngine.Scheduling.Coroutines;

namespace SFML_tutorial.Games.TownTest.Entities;
public class Player : Moveable
{
    private readonly Dictionary<Key, bool> pressedKeys = KeysToPressedDict([Key.W, Key.A, Key.S, Key.D]);
    private Color PlayerColor { get; set; } = Color.Red;

    public required Vector2f Size { get; set; }
    public required override Vector2f Position { get => base.Position; set => base.Position = value; }

    public override List<Drawable> Drawables => [new RectangleShape
    {
        FillColor = PlayerColor,
        Position = Position,
        Size = Size,
    }];
    public override Collider2D Collider => new Collider2D
    {
        IsStatic = false,
        PositionableGameObject = this,
        Bounds = new FloatRect(Position, Size),
    };

    public override void Attach()
    {
        GameWindow.Instance.RenderWindow.KeyPressed += HandleKeyPress;
        GameWindow.Instance.RenderWindow.KeyReleased += HandleKeyRelease;

        StartCoroutine(ColorChange());
    }

    private IEnumerator ColorChange()
    {
        PlayerColor = Color.Green;
        yield return new WaitForSeconds(2f);
        PlayerColor = Color.Cyan;
        yield return null;
        PlayerColor = Color.Red;
    }

    public override void OnDestroy()
    {
        GameWindow.Instance.RenderWindow.KeyPressed -= HandleKeyPress;
        GameWindow.Instance.RenderWindow.KeyReleased -= HandleKeyRelease;
    }

    public override void Update()
    {
        Move(new Vector2f(-pressedKeys[Key.A].ToInt() + pressedKeys[Key.D].ToInt(), -pressedKeys[Key.W].ToInt() + pressedKeys[Key.S].ToInt()));
        // PlayerColor = ColorExtensions.PingPong(Color.Red, Color.Blue, GameWindow.Time.AsSeconds(), 2);

        // track linearly, can smoothstep the Position for every frame we update the center or whatever other follow behaviour one might prefer
        GameWindow.Instance.MainView.Center = Position;
    }
    private void HandleKeyPress(object? sender, KeyEventArgs e)
    {
        if (pressedKeys.ContainsKey(e.Code))
        {
            pressedKeys[e.Code] = true;
        }
    }
    private void HandleKeyRelease(object? sender, KeyEventArgs e)
    {
        if (pressedKeys.ContainsKey(e.Code))
        {
            pressedKeys[e.Code] = false;
        }
    }

}
