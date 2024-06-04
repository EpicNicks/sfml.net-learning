﻿using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.BaseEngine.CoreLibs.Composed;

/// <summary>
/// A sort of trait class for types interested in being anchored to different screen segments to inherit
/// </summary>
public abstract class UIAnchored: Positionable
{
    public enum UIAnchor
    {
        START,
        CENTER,
        END
    }
    public virtual (UIAnchor x, UIAnchor y) Anchors { get; set; } = (UIAnchor.START, UIAnchor.START);

    protected Vector2f PositionLocally(FloatRect bounds)
    {
        float xAnchor = Anchors.x switch
        {
            UIAnchor.START => 0,
            UIAnchor.CENTER => (GameWindow.Instance.UiView.Size.X - bounds.Width) / 2f,
            UIAnchor.END => GameWindow.Instance.UiView.Size.X - bounds.Width,
            _ => 0,
        };
        float yAnchor = Anchors.y switch
        {
            UIAnchor.START => 0,
            UIAnchor.CENTER => (GameWindow.Instance.UiView.Size.Y - bounds.Height) / 2f,
            UIAnchor.END => GameWindow.Instance.UiView.Size.Y - bounds.Height,
            _ => 0,
        };

        return Position + new Vector2f(xAnchor, yAnchor);
    }
}
