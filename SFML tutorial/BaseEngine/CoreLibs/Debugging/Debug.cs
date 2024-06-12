using System.Collections;

using SFML.Graphics;
using SFML.System;

using SFML_tutorial.BaseEngine.GameObjects.Composed;
using SFML_tutorial.BaseEngine.Scheduling.Coroutines;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.BaseEngine.CoreLibs.Debugging;
public static class Debug
{
    private class DebugLineSegment : GameObject
    {
        private readonly float secondsToLive;
        private readonly VertexArray lineSegment;

        public DebugLineSegment(Vector2f point1, Vector2f point2, float secondsToLive = 0f)
        {
            this.secondsToLive = secondsToLive;
            lineSegment = new VertexArray(PrimitiveType.Lines, 2);
            lineSegment[0] = new Vertex(point1, Color.Green);
            lineSegment[1] = new Vertex(point2, Color.Green);
        }

        public override List<Drawable> Drawables => [lineSegment];

        public override void Attach()
        {
            StartCoroutine(DestroyInSeconds());
        }

        private IEnumerator DestroyInSeconds()
        {
            yield return new WaitForSeconds(secondsToLive);
            Destroy();
        }
    }

    public static void DrawLineSegment(Vector2f point1, Vector2f point2, float secondsToLive = 0f)
    {
        GameWindow.Add(RenderLayer.BASE, new DebugLineSegment(point1, point2, secondsToLive));
    }
}
