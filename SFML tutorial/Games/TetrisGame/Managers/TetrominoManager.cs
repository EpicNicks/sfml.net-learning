using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.GameObjects.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.TetrisGame.Entities;
using System.Linq;

namespace SFML_tutorial.Games.TetrisGame.Managers;
public class TetrominoManager : GameObject
{
    private readonly Random rnd;

    private List<Tetromino> placedTetrominoes = [];
    private Tetromino? activeTetromino;
    private Queue<Tetromino> nextTetrominoes = [];
    /// <summary>
    /// Can be decreased over time to increase game speed
    /// </summary>
    private float movesPerSecond;
    private float moveSecondsElapsed;
    private bool activeTetrominoWasTouchingLastFrame = false;

    public TetrominoManager()
    {
        rnd = new Random();
    }

    public override void Attach()
    {
        movesPerSecond = 1.0f;
        foreach (var _ in Enumerable.Range(0, 10))
        {
            nextTetrominoes.Enqueue(Tetromino.Random(rnd));
        }
        GameWindow.Add(RenderLayer.BASE, new TetrisFloor(new Vector2f(TetrisMain.GAME_WIDTH / 2f, 10)) { Position = new Vector2f(TetrisMain.GAME_WIDTH / 4f, TetrisMain.GAME_HEIGHT - 100)});
    }

    public override void Update()
    {
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
            if (placedTetrominoes.Any(activeTetromino.IsColliding) || activeTetromino.Drawables.Any(d =>
            {
                if (d is RectangleShape r)
                {
                    return GameWindow.FindObjectOfType<TetrisFloor>()?.Drawables.Any(floorDrawable =>
                    {
                        if (floorDrawable is RectangleShape floorRectShape)
                        {
                            return r.GetGlobalBounds().Intersects(floorRectShape.GetGlobalBounds());
                        }
                        return false;
                    }) ?? false;
                }
                return false;
            }))
            {
                // undo move
                activeTetromino.Position -= AdvanceTetrominoDelta();
                if (activeTetrominoWasTouchingLastFrame)
                {
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

    private static Vector2f AdvanceTetrominoDelta()
        => new(0, Tetromino.BLOCK_SIZE + Tetromino.BLOCK_MARGIN / 2f);
}
