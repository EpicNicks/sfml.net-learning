using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.TetrisGame.Managers;

namespace SFML_tutorial.Games.TetrisGame;
public class TetrisMain
{
    public const int GAME_WIDTH = 1200;
    public const int GAME_HEIGHT = 800;
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
