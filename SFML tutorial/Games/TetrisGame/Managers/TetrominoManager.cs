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
using System.Collections;
using SFML_tutorial.BaseEngine.Scheduling.Coroutines;
using SFML_tutorial.Games.TetrisGame.UI;
using SFML_tutorial.Games.TetrisGame.Helpers;

namespace SFML_tutorial.Games.TetrisGame.Managers;
public class TetrominoManager : GameObject
{
    private readonly Random rnd;
    /// <summary>
    /// Keys which can be held
    /// </summary>
    private readonly Dictionary<Key, bool> keysPressedDict = Moveable.KeysToPressedDict([Key.Left, Key.A, Key.Right, Key.D, Key.S, Key.Down]);

    private List<RectangleShape> placedRectangles = [];
    private Tetromino? activeTetromino;
    private Queue<Tetromino> nextTetrominoes = [];
    /// <summary>
    /// Can be decreased over time to increase game speed
    /// </summary>
    private float baseMovesPerSecond = 1.0f;
    private float movesPerSecond;
    private float moveSecondsElapsed;
    private readonly int FRAMES_BEFORE_TETROMINO_PLACED = 1;
    private int activeTetrominoPlacedFrames = 0;
    private bool isGameOver = false;
    private bool isPaused;
    private bool isWaitingForAnimation = false;

    private ScoreText? scoreText;
    private UIAnchoredText? pauseText;
    private UIAnchoredText? gameOverText;

    private static Vector2f AdvanceTetrominoDelta => new(0, Tetromino.BLOCK_SIZE);

    public TetrominoManager()
    {
        rnd = new Random();
    }

    public override List<Drawable> Drawables => [.. placedRectangles.Select(r => (Drawable)r).ToList() ];

    public override void Attach()
    {
        scoreText = GameWindow.FindObjectOfType<ScoreText>();
        pauseText = GameWindow.FindObjectOfType<UIAnchoredText>("Pause Text");
        gameOverText = GameWindow.FindObjectOfType<UIAnchoredText>("Game Over Text");

        if (pauseText is not null)
        {
            pauseText.IsActive = false;
        }
        if (gameOverText is not null)
        {
            gameOverText.IsActive = false;
        }

        movesPerSecond = baseMovesPerSecond * 0.5f;
        RefillBag();
        RegisterInputEvents();
        StartCoroutine(HandleInput());
    }

    public override void OnDestroy()
    {
        UnregisterInputEvents();
    }

    public override void Update()
    {
        if (pauseText is not null)
        {
            pauseText.IsActive = isPaused;
        }
        UpdateTetrominoes();
        DrawDebugBounds();
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
        else if (keyEvent.Code == Key.E)
        {
            RotateActiveTetromino();
        }
        else if (keyEvent.Code == Key.Q)
        {
            RotateActiveTetromino(false);
        }
        else if (keyEvent.Code == Key.P)
        {
            if (!isGameOver)
            {
                isPaused = !isPaused;
            }
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
        if (isWaitingForAnimation)
        {
            return;
        }
        if (isPaused)
        {
            return;
        }
        if (activeTetromino is null)
        {
            activeTetromino = nextTetrominoes.Dequeue();
            activeTetromino.Position = new(TetrisMain.GAME_WIDTH / 2f, TetrisMain.CEILING_HEIGHT - Tetromino.BLOCK_SIZE * 4);
            GameWindow.Add(RenderLayer.BASE, activeTetromino);
            RefillBag();
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
            activeTetromino.Position += AdvanceTetrominoDelta;
            if (activeTetromino.IsColliding(placedRectangles) || TetrominoIsTouchingFloor(activeTetromino))
            {
                // undo move
                activeTetromino.Position -= AdvanceTetrominoDelta;
                if (activeTetrominoPlacedFrames >= FRAMES_BEFORE_TETROMINO_PLACED)
                {
                    // place Tetromino
                    if (TetrominoIsPlacedAboveCieling(activeTetromino))
                    {
                        isGameOver = true;
                        if (gameOverText is not null)
                        {
                            gameOverText.IsActive = true;
                        }
                    }
                    placedRectangles.AddRange(activeTetromino.Rectangles.Select(r =>
                    {
                        r.Position = new((int)System.Math.Round(r.Position.X), (int)System.Math.Round(r.Position.Y));
                        return r;
                    }));
                    activeTetrominoPlacedFrames = 0;
                    activeTetromino.Destroy();
                    activeTetromino = null;
                    ClearCompleteRows();
                }
                else
                {
                    activeTetrominoPlacedFrames++;
                }
            }
        }
    }

    private void DrawDebugBounds()
    {
        Color lineColor = new Color(0x999999FF);

        Debug.DrawLineSegment(new(TetrisMain.LEFT_WALL_POS, TetrisMain.CEILING_HEIGHT), new(TetrisMain.RIGHT_WALL_POS, TetrisMain.CEILING_HEIGHT), lineColor);
        Debug.DrawLineSegment(new(TetrisMain.LEFT_WALL_POS, TetrisMain.FLOOR_HEIGHT), new(TetrisMain.RIGHT_WALL_POS, TetrisMain.FLOOR_HEIGHT), lineColor);
        Debug.DrawLineSegment(new(TetrisMain.LEFT_WALL_POS, TetrisMain.FLOOR_HEIGHT), new(TetrisMain.LEFT_WALL_POS, TetrisMain.CEILING_HEIGHT), lineColor);
        Debug.DrawLineSegment(new(TetrisMain.RIGHT_WALL_POS, TetrisMain.FLOOR_HEIGHT), new(TetrisMain.RIGHT_WALL_POS, TetrisMain.CEILING_HEIGHT), lineColor);
        // grid lines
        //  vertical
        for (float f = TetrisMain.LEFT_WALL_POS; f < TetrisMain.RIGHT_WALL_POS; f += Tetromino.BLOCK_SIZE)
        {
            Debug.DrawLineSegment(new(f, TetrisMain.FLOOR_HEIGHT), new(f, TetrisMain.CEILING_HEIGHT), lineColor);
        }
        //  horizontal
        for (float f = TetrisMain.FLOOR_HEIGHT; f > TetrisMain.CEILING_HEIGHT; f -= Tetromino.BLOCK_SIZE)
        {
            Debug.DrawLineSegment(new(TetrisMain.LEFT_WALL_POS, f), new(TetrisMain.RIGHT_WALL_POS, f), lineColor);
        }
    }

    private IEnumerator HandleInput()
    {
        while (true)
        {
            if (!isPaused)
            {
                float lrInput = -keysPressedDict[Key.A].ToInt() + -keysPressedDict[Key.Left].ToInt() + keysPressedDict[Key.D].ToInt() + keysPressedDict[Key.Right].ToInt();
                if (activeTetromino is not null)
                {
                    if (lrInput != 0)
                    {
                        activeTetromino.Position += new Vector2f(Tetromino.BLOCK_SIZE * lrInput, 0);
                        if (!TetrominoInValidPosition(activeTetromino))
                        {
                            activeTetromino.Position -= new Vector2f(Tetromino.BLOCK_SIZE * lrInput, 0);
                        }
                    }
                    if (keysPressedDict[Key.S] || keysPressedDict[Key.Down])
                    {
                        movesPerSecond = baseMovesPerSecond * 0.01f;
                    }
                    else
                    {
                        movesPerSecond = baseMovesPerSecond * 0.5f;
                    }
                }
            }
            // input wait to have breathing room every frame
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void RotateActiveTetromino(bool turnRight = true)
    {
        if (isPaused)
        {
            return;
        }
        if (activeTetromino is not null)
        {
            // TODO: try rotate, check collisions if there is a collision, unrotate
            if (turnRight)
            {
                activeTetromino.TurnRight();
            }
            else
            {
                activeTetromino.TurnLeft();
            }
            if (!TetrominoInValidPosition(activeTetromino))
            {
                // undo turn
                if (turnRight)
                {
                    activeTetromino.TurnLeft();
                }
                else
                {
                    activeTetromino.TurnRight();
                }
            }
        }
    }

    /// <summary>
    /// Checks for any rows that are complete and clears them
    /// </summary>
    private void ClearCompleteRows()
    {
        Console.WriteLine();
        Dictionary<int, List<RectangleShape>> rows = [];
        foreach (RectangleShape rectangle in placedRectangles)
        {
            int roundedYPosition = (int)System.Math.Round(rectangle.Position.Y);
            if (rows.TryGetValue(roundedYPosition, out var rects))
            {
                rects.Add(rectangle);
            }
            else
            {
                rows[roundedYPosition] = [rectangle];
            }
        }

        // all rows above the highest cleared row go to where the lowest cleared row was
        int lowestRow = 0; // highest Y value
        List<int> rowsToClear = [];
        List<RectangleShape> rectanglesToRemove = [];
        foreach (int roundedYPos in rows.Keys)
        {
            if (rows[roundedYPos].Count == 10)
            {
                rowsToClear.Add(roundedYPos);
                if (roundedYPos > lowestRow)
                {
                    lowestRow = roundedYPos;
                }
                foreach (RectangleShape rect in rows[roundedYPos])
                {
                    rectanglesToRemove.Add(rect);
                }
            }
        }

        IEnumerator ClearRowsAnimation()
        {
            Queue<Color> originalColors = [];
            foreach (RectangleShape rect in rectanglesToRemove)
            {
                originalColors.Enqueue(rect.FillColor);
                rect.FillColor = Color.White;
            }
            yield return new WaitForSeconds(0.1f);
            foreach (RectangleShape rect in rectanglesToRemove)
            {
                rect.FillColor = originalColors.Dequeue();
            }
            yield return new WaitForSeconds(0.1f);
            foreach (RectangleShape rect in rectanglesToRemove)
            {
                rect.FillColor = Color.White;
            }
            yield return new WaitForSeconds(0.1f);

            foreach (RectangleShape rect in rectanglesToRemove)
            {
                placedRectangles.Remove(rect);
            }

            foreach (RectangleShape rectangleShape in placedRectangles)
            {
                if ((int)System.Math.Round(rectangleShape.Position.Y) < lowestRow)
                {
                    int numberOfRowsBelowRect = rowsToClear.Count(roundedYPos => roundedYPos > rectangleShape.Position.Y);
                    rectangleShape.Position = new Vector2f(rectangleShape.Position.X, rectangleShape.Position.Y + numberOfRowsBelowRect * Tetromino.BLOCK_SIZE);
                }
            }
            if (scoreText is not null)
            {
                scoreText.Score += rowsToClear.Count switch
                {
                    0 => 0,
                    1 => 40,
                    2 => 100,
                    3 => 300,
                    4 => 1200,
                    _ => throw new InvalidOperationException($"Number of rows to clear: ({rowsToClear.Count}) not in range [1,4]")
                };
            }
        }
        StartCoroutine(ClearRowsAnimation());
    }

    private void RefillBag()
    {
        if (nextTetrominoes.Count == 0)
        {
            foreach (Tetromino tetromino in TetrominoHelper.GenerateBag(rnd))
            {
                nextTetrominoes.Enqueue(tetromino);
            };
        }
    }

    /// <summary>
    /// Used to check if the move just performed was valid
    /// </summary>
    /// <returns>true if the current position is not overlapping with other game pieces, false otherwise</returns>
    private bool TetrominoInValidPosition(Tetromino toTest)
    {
        static bool IsBelowFloor(Transformable t) => (t.Position.Y + Tetromino.BLOCK_SIZE) > TetrisMain.FLOOR_HEIGHT;
        static bool IsBeyondLeftWall(Transformable t) => t.Position.X < TetrisMain.LEFT_WALL_POS;
        static bool IsBeyondRightWall(Transformable t) => (t.Position.X + Tetromino.BLOCK_SIZE) > TetrisMain.RIGHT_WALL_POS;
        return toTest.Drawables.All(d => d is Transformable t && (
            !IsBelowFloor(t) && !IsBeyondLeftWall(t) && !IsBeyondRightWall(t)
        )) && !toTest.IsColliding(placedRectangles);
    }

    private static bool TetrominoIsPlacedAboveCieling(Tetromino tetrominoToTest)
        => tetrominoToTest.Drawables.Any(d => d is Transformable t && t.Position.Y < TetrisMain.CEILING_HEIGHT);

    private static bool TetrominoIsTouchingFloor(Tetromino tetrominoToTest)
        => tetrominoToTest.Drawables.Any(d => d is RectangleShape r && r.Position.Y >= TetrisMain.FLOOR_HEIGHT);

}
