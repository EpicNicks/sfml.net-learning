using SFML.Audio;

using SFML_tutorial.BaseEngine.GameObjects.Composed;

namespace SFML_tutorial.Games.TetrisGame.Managers;
public class MusicManager : GameObject
{
    Music? music;

    public override void Attach()
    {
        music = new Music("Resources/Tetris Theme.flac")
        {
            Loop = true,
        };
        music.Play();
    }
}
