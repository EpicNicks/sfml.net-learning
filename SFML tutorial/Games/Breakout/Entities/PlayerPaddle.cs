using SFML.Graphics;
using SFML.Window;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;
using SFML_tutorial.BaseEngine.Scheduling.Coroutines;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.Breakout.UI;
using System.Collections;
using static SFML.Window.Keyboard;

namespace SFML_tutorial.Games.Breakout.Entities;
public class PlayerPaddle : Moveable
{
    private readonly RectangleShape rectangleShape;
    private readonly Collider2D collider;
    private readonly Dictionary<Key, bool> pressedKeys = KeysToPressedDict([Key.A, Key.D]);

    private ScoreText? scoreText;

    public PlayerPaddle(FloatRect bounds)
    {
        Position = new(bounds.Left, bounds.Top);
        rectangleShape = new RectangleShape
        {
            FillColor = Color.White,
            Position = Position,
            Size = new(bounds.Width, bounds.Height)
        };
        collider = new()
        {
            IsStatic = false,
            PositionableGameObject = this,
            Bounds = bounds,
            IsTrigger = false
        };
    }

    public override void Attach()
    {
        scoreText = GameWindow.FindObjectOfType<ScoreText>();
        StartCoroutine(TestScore());
        GameWindow.Instance.RenderWindow.KeyPressed += OnKeyPressed;
        GameWindow.Instance.RenderWindow.KeyReleased += OnKeyReleased;
    }

    public override void OnDestroy()
    {
        GameWindow.Instance.RenderWindow.KeyPressed -= OnKeyPressed;
        GameWindow.Instance.RenderWindow.KeyReleased -= OnKeyReleased;
    }

    public override void Update()
    {
        Move(new(-pressedKeys[Key.A].ToInt() + pressedKeys[Key.D].ToInt(), 0));
        Position = Position.ClampX(0, 1200 - rectangleShape.Size.X);
    }

    private IEnumerator TestScore()
    {
        while (true)
        {
            if (scoreText is not null)
            {
                yield return new WaitForSeconds(3);
                scoreText.Score += 5;
            }
        }
    }

    public override List<Drawable> Drawables => [rectangleShape];
    public override Collider2D Collider => collider;

    private void OnKeyPressed(object? _, KeyEventArgs key)
    {
        if (pressedKeys.ContainsKey(key.Code))
        {
            pressedKeys[key.Code] = true;
        }
    }
    private void OnKeyReleased(object? _, KeyEventArgs key)
    {
        if (pressedKeys.ContainsKey(key.Code))
        {
            pressedKeys[key.Code] = false;
        }
    }
}
