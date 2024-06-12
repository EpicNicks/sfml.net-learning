using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.TetrisGame.Managers;

namespace SFML_tutorial.Games.TetrisGame;
public class TetrisMain
{
    public const int GAME_WIDTH = 1200;
    public const int GAME_HEIGHT = 800;
    public const int CEILING_HEIGHT = 200;
    public const int FLOOR_HEIGHT = GAME_HEIGHT - 100;
    public const int LEFT_WALL_POS = 200;
    public const int RIGHT_WALL_POS = GAME_WIDTH - LEFT_WALL_POS;
    public static void Run()
    {
        GameWindow.WindowTitle = "Tetris";
        GameWindow.Size = new(GAME_WIDTH, GAME_HEIGHT);

        GameWindow.AddScene(Scene1);

        GameWindow.Run();
    }

    private static Scene Scene1 => new Scene("Scene 1", add =>
    {
        add((RenderLayer.NONE, new TetrominoManager()));
    });
}
