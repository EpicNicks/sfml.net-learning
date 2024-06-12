using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard;

using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.GameObjects.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.TetrisGame.Entities;
using SFML_tutorial.BaseEngine.CoreLibs.Debugging;
using SFML_tutorial.BaseEngine.CoreLibs.Mathematics;

namespace SFML_tutorial.Games.TetrisGame.Managers;
public class TetrominoManager : GameObject
{
    private readonly Random rnd;
    private readonly Dictionary<Key, bool> keysPressedDict = Moveable.KeysToPressedDict([Key.Left, Key.A, Key.Right, Key.D]);

    private List<Tetromino> placedTetrominoes = [];
    private Tetromino? activeTetromino;
    private Queue<Tetromino> nextTetrominoes = [];
    /// <summary>
    /// Can be decreased over time to increase game speed
    /// </summary>
    private float movesPerSecond;
    private float moveSecondsElapsed;
    private bool activeTetrominoWasTouchingLastFrame = false;
    private bool isGameOver = false;

    public TetrominoManager()
    {
        rnd = new Random();
    }

    public override void Attach()
    {
        movesPerSecond = 0.1f;
        foreach (var _ in Enumerable.Range(0, 10))
        {
            nextTetrominoes.Enqueue(Tetromino.Random(rnd));
        }
        RegisterInputEvents();
    }

    public override void OnDestroy()
    {
        UnregisterInputEvents();
    }

    public override void Update()
    {
        UpdateTetrominoes();
        HandleInput();
        Debug.DrawLineSegment(new(TetrisMain.LEFT_WALL_POS, TetrisMain.FLOOR_HEIGHT), new(TetrisMain.RIGHT_WALL_POS, TetrisMain.FLOOR_HEIGHT));
        Debug.DrawLineSegment(new(TetrisMain.LEFT_WALL_POS, TetrisMain.FLOOR_HEIGHT), new(TetrisMain.LEFT_WALL_POS, TetrisMain.CEILING_HEIGHT));
        Debug.DrawLineSegment(new(TetrisMain.RIGHT_WALL_POS, TetrisMain.FLOOR_HEIGHT), new(TetrisMain.RIGHT_WALL_POS, TetrisMain.CEILING_HEIGHT));
    }

    private void RegisterInputEvents()
    {
        GameWindow.Instance.RenderWindow.KeyPressed += OnKeyPressed;
        GameWindow.Instance.RenderWindow.KeyReleased += OnKeyReleased;
    }

    private void UnregisterInputEvents()
    {
        GameWindow.Instance.RenderWindow.KeyPressed -= OnKeyPressed;
        GameWindow.Instance.RenderWindow.KeyReleased -= OnKeyReleased;
    }

    private void OnKeyPressed(object? _, KeyEventArgs keyEvent)
    {
        if (keysPressedDict.ContainsKey(keyEvent.Code))
        {
            keysPressedDict[keyEvent.Code] = true;
        }
    }
    private void OnKeyReleased(object? _, KeyEventArgs keyEvent)
    {
        if (keysPressedDict.ContainsKey(keyEvent.Code))
        {
            keysPressedDict[keyEvent.Code] = false;
        }
    }

    private void UpdateTetrominoes()
    {
        if (isGameOver)
        {
            return;
        }
        if (activeTetromino is null)
        {
            activeTetromino = nextTetrominoes.Dequeue();
            activeTetromino.Position = new(TetrisMain.GAME_WIDTH / 2f, 0);
            GameWindow.Add(RenderLayer.BASE, activeTetromino);
            nextTetrominoes.Enqueue(Tetromino.Random(rnd));
        }
        else
        {
            moveSecondsElapsed += GameWindow.DeltaTime.AsSeconds();
            if (moveSecondsElapsed < movesPerSecond)
            {
                return;
            }
            moveSecondsElapsed = 0;
            // try move
            activeTetromino.Position += AdvanceTetrominoDelta();
            if (placedTetrominoes.Any(activeTetromino.IsColliding) || TetrominoIsTouchingFloor(activeTetromino))
            {
                // undo move
                activeTetromino.Position -= AdvanceTetrominoDelta();
                if (activeTetrominoWasTouchingLastFrame)
                {
                    if (TetrominoIsPlacedAboveCieling(activeTetromino))
                    {
                        isGameOver = true;
                    }
                    placedTetrominoes.Add(activeTetromino);
                    activeTetromino = null;
                }
                else
                {
                    activeTetrominoWasTouchingLastFrame = true;
                }
            }
        }
    }

    private void HandleInput()
    {
        // might want an input step (cooldown) so pressing and holding doesn't move like crazy
        float lrInput = -keysPressedDict[Key.A].ToInt() + -keysPressedDict[Key.Left].ToInt() + keysPressedDict[Key.D].ToInt() + keysPressedDict[Key.Right].ToInt();
        if (activeTetromino is not null && lrInput != 0)
        {
            activeTetromino.Position += new Vector2f(Tetromino.BLOCK_SIZE * lrInput, 0);
        }
    }

    private static bool TetrominoIsPlacedAboveCieling(Tetromino tetrominoToTest)
        => tetrominoToTest.Drawables.Any(d => d is Transformable t && t.Position.Y < TetrisMain.CEILING_HEIGHT);

    private static bool TetrominoIsTouchingFloor(Tetromino tetrominoToTest)
        => tetrominoToTest.Drawables.Any(d => d is RectangleShape r && (r.Position.Y + Tetromino.BLOCK_SIZE + Tetromino.BLOCK_MARGIN) >= TetrisMain.FLOOR_HEIGHT);
        //=> tetrominoToTest.Drawables.Any(d => d is RectangleShape r && (
        //        GameWindow.FindObjectOfType<TetrisFloor>()?.Drawables.Any(floorDrawable =>
        //            floorDrawable is RectangleShape floorRectShape && r.GetGlobalBounds().Intersects(floorRectShape.GetGlobalBounds())
        //        ) ?? false
        //    )
        //);

    private static Vector2f AdvanceTetrominoDelta()
        => new(0, Tetromino.BLOCK_SIZE + Tetromino.BLOCK_MARGIN / 2f);
}
