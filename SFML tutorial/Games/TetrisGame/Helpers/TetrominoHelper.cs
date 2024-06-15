using SFML_tutorial.BaseEngine.CoreLibs.SFMLExtensions;
using SFML_tutorial.Games.TetrisGame.Entities;

namespace SFML_tutorial.Games.TetrisGame.Helpers;
public static class TetrominoHelper
{
    /// <summary>
    /// Generates the 7 Tetrominoes in a random order.
    /// </summary>
    /// <returns>Each Tetromino in a random order</returns>
    public static Tetromino[] GenerateBag(Random rnd)
    {
        return AllTetrominoes()
            .Select(tetromino =>
            {
                tetromino.Color = ColorExtensions.RandomBrightColor(rnd);
                return tetromino;
            })
            .OrderBy(_ => rnd.Next())
            .ToArray();
    }

    /// <summary>
    /// Creates a collection of Tetrominoes in the same order
    /// </summary>
    /// <returns>[I, T, O, L, J, S, Z]</returns>
    public static Tetromino[] AllTetrominoes()
        => [Tetromino.I, Tetromino.T, Tetromino.O, Tetromino.L, Tetromino.J, Tetromino.S, Tetromino.Z];
}
