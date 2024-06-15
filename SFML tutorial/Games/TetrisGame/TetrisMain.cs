using SFML.Graphics;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.TetrisGame.UI;
using SFML_tutorial.Games.TetrisGame.Entities;
using SFML_tutorial.Games.TetrisGame.Managers;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.Properties;
using SFML.Audio;

namespace SFML_tutorial.Games.TetrisGame;
public class TetrisMain
{
    public const int GAME_WIDTH = 1200;
    public const int GAME_HEIGHT = 1080;
    public const int CEILING_HEIGHT = -100;
    public const int FLOOR_HEIGHT = Tetromino.BLOCK_SIZE * 40 - 100;
    public const int BOARD_WIDTH = Tetromino.BLOCK_SIZE * 10;
    public const int LEFT_WALL_POS = GAME_WIDTH / 2 - BOARD_WIDTH / 2;
    public const int RIGHT_WALL_POS = LEFT_WALL_POS + BOARD_WIDTH;
    public static void Run()
    {
        GameWindow.WindowTitle = "Tetris";
        GameWindow.WindowBackgroundColor = Color.Black;
        GameWindow.Size = new(GAME_WIDTH, GAME_HEIGHT);

        GameWindow.AddScene(Scene1);

        GameWindow.Run();
    }

    private static Scene Scene1 => new Scene("Scene 1", add =>
    {
        add((RenderLayer.NONE, new MusicManager()));
        add((RenderLayer.BASE, new TetrominoManager()));
        add((RenderLayer.UI, new ScoreText()));
        add((RenderLayer.UI, new UIAnchoredText("Paused", new Font(Resources.Roboto_Black), Color.White, 48)
        {
            Anchors = (UIAnchored.UIAnchor.CENTER, UIAnchored.UIAnchor.CENTER),
            IsActive = false,
            Name = "Pause Text"
        }));
        add((RenderLayer.UI, new UIAnchoredText("Game Over", new Font(Resources.Roboto_Black), Color.White, 60)
        {
            Anchors = (UIAnchored.UIAnchor.CENTER, UIAnchored.UIAnchor.CENTER),
            IsActive = false,
            Name = "Game Over Text"
        }));
    });
}
