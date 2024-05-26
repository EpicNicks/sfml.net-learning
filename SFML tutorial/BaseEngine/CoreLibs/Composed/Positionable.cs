﻿using SFML.Graphics;
using SFML.System;

using SFML_tutorial.BaseEngine.GameObjects.Composed;

namespace SFML_tutorial.BaseEngine.CoreLibs.Composed;

public class Positionable: GameObject
{
    private Vector2f position = new Vector2f();
    public virtual Vector2f Position 
    {
        get => position;
        set
        {
            Vector2f delta = value - Position;
            foreach (Drawable d in Drawables)
            {
                if (d is Transformable t)
                {
                    t.Position += delta;
                }
            }
            if (Collider != null)
            {
                Collider.Bounds = new FloatRect(new Vector2f(Collider.Bounds.Left, Collider.Bounds.Top) + delta, new Vector2f(Collider.Bounds.Width, Collider.Bounds.Height));
            }
            position = value;
        }
    }
}
