using System.Collections.ObjectModel;
using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.CoreLibs.SFMLExtensions;

namespace SFML_tutorial.Games.TetrisGame.Entities;

public sealed class Tetromino : Positionable
{
    /// <summary>
    /// The size of rendered RectangleShapes
    /// </summary>
    public const int BLOCK_SIZE = 20;
    /// <summary>
    /// The space between rendered RectangleShapes
    /// </summary>
    public const int BLOCK_MARGIN = 1;

    private readonly uint shapeIndex;
    public uint Turns { get; private set; }
    public Color Color { get; set; }

    private static readonly ushort[,] shapes = new ushort[7,4]
    {
        { 0x4640, 0x0E40, 0x4C40, 0x4E00 }, // T
        { 0x8C40, 0x6C00, 0x8C40, 0x6C00 }, // S
        { 0x4C80, 0xC600, 0x4C80, 0xC600 }, // Z
        { 0x4444, 0x0F00, 0x4444, 0x0F00 }, // Straight
        { 0x44C0, 0x8E00, 0xC880, 0xE200 }, // J
        { 0x88C0, 0xE800, 0xC440, 0x2E00 }, // L
        { 0xCC00, 0xCC00, 0xCC00, 0xCC00 }  // BOX
    };


    private Tetromino(uint shapeIndex)
    {
        this.shapeIndex = shapeIndex;
    }

    public Tetromino(Tetromino other) : this(other.shapeIndex)
    {
        Turns = other.Turns;
        Color = other.Color;
    }

    public void TurnLeft()
    {
        if (Turns == 0)
        {
            Turns = 3;
        }
        else
        {
            Turns--;
        }
    }

    public void TurnRight()
    {
        Turns = (Turns + 1) % 4;
    }

    #region Public API for retrieving Tetrominoes
    public static Tetromino T => new(0);
    public static Tetromino S => new(1);
    public static Tetromino Z => new(2);
    public static Tetromino Straight => new(3);
    public static Tetromino J => new(4);
    public static Tetromino L => new(5);
    public static Tetromino Box => new(6);
    public static Tetromino Random(Random rnd) => new((uint)rnd.Next(0, 7)) { Color = ColorExtensions.Random(rnd) };
    #endregion

    public override List<Drawable> Drawables
    {
        get 
        {
            List<Drawable> shapesRet = [];

            ushort bitmask = shapes[shapeIndex, Turns];
            for (int i = 0; i < 16; i++)
            {
                if ((bitmask & (0x8000 >> i)) != 0)
                {
                    int x = i % 4;
                    int y = i / 4;
                    shapesRet.Add(new RectangleShape(new Vector2f(BLOCK_SIZE - BLOCK_MARGIN * 2, BLOCK_SIZE - BLOCK_MARGIN * 2))
                    {
                        FillColor = Color, // Set the color of the tetromino block
                        Position = Position + new Vector2f(x * BLOCK_SIZE, y * BLOCK_SIZE),
                        OutlineColor = Color.White,
                        OutlineThickness = BLOCK_MARGIN,
                    });
                    if (shapesRet.Count == 4) break; // early break, stop adding blocks once we have four
                }
            }
            return shapesRet;
        }
    }

    public List<RectangleShape> Rectangles
    {
        get
        {
            List<RectangleShape> shapesRet = [];

            ushort bitmask = shapes[shapeIndex, Turns];
            for (int i = 0; i < 16; i++)
            {
                if ((bitmask & (0x8000 >> i)) != 0)
                {
                    int x = i % 4;
                    int y = i / 4;
                    shapesRet.Add(new RectangleShape(new Vector2f(BLOCK_SIZE - BLOCK_MARGIN * 2, BLOCK_SIZE - BLOCK_MARGIN * 2))
                    {
                        FillColor = Color, // Set the color of the tetromino block
                        Position = Position + new Vector2f(x * BLOCK_SIZE, y * BLOCK_SIZE),
                        OutlineColor = Color.White,
                        OutlineThickness = BLOCK_MARGIN,
                    });
                    if (shapesRet.Count == 4) break; // early break, stop adding blocks once we have four
                }
            }
            return shapesRet;
        }
    }

    public bool IsColliding(Tetromino other)
    {
        return IsColliding(other.Rectangles);
    }

    public bool IsColliding(List<RectangleShape> rectangleShapes)
    {
        foreach (RectangleShape thisRectangle in Rectangles)
        {
            foreach (RectangleShape otherRectangleShape in rectangleShapes)
            {
                if (thisRectangle.GetGlobalBounds().Intersects(otherRectangleShape.GetGlobalBounds()))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
