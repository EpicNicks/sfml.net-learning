using SFML_tutorial.BaseEngine.GameObjects.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using SFML_tutorial.Games.Breakout.Entities;

namespace SFML_tutorial.Games.PongGame.StateManagers;
/// <summary>
/// Persistant GameObject which handles changing scenes when the Bricks are all cleared from the current one
/// </summary>
public class BrickManager : GameObject
{
    public override void Attach()
    {
        // entering a scene that doesn't have bricks (a victory screen or something)
        if (GameWindow.FindObjectsOfType<Brick>().Count == 0)
        {
            Destroy();
        }
    }

    public override void Update()
    {
        if (GameWindow.FindObjectsOfType<Brick>().Count == 0)
        {
            if (GameWindow.HasNextScene())
            {
                GameWindow.LoadNextScene();
            }
        }
    }
}
