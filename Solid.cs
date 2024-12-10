using System.Collections.Generic;
using UnityEngine;

public class Solid : MonoBehaviour
{
    public static List<Solid> AllSolids = new List<Solid>();
    public Vector2 Position { get; private set; }
    private float xRemainder;
    private float yRemainder;
    public Rect Collider; // Rect representing the bounding box of the Solid
    public bool Collidable { get; private set; } = true;

    private void Awake()
    {
        AllSolids.Add(this);
    }

    public void Initialize(Vector2 initialPosition, Vector2 colliderSize)
    {
        Position = initialPosition;
        Collider = new Rect(initialPosition, colliderSize);
    }

    public void Move(float x, float y)
    {
        xRemainder += x;
        yRemainder += y;
        int moveX = Mathf.RoundToInt(xRemainder);
        int moveY = Mathf.RoundToInt(yRemainder);

        if (moveX != 0 || moveY != 0)
        {
            xRemainder -= moveX;
            yRemainder -= moveY;

            List<Actor> ridingActors = GetRidingActors();
            Collidable = false;

            if (moveX != 0)
                MoveAxis(moveX, ridingActors, true);
            if (moveY != 0)
                MoveAxis(moveY, ridingActors, false);

            Collidable = true;
        }
    }

    private void MoveAxis(int move, List<Actor> ridingActors, bool isHorizontal)
    {
        Vector2 delta = isHorizontal ? new Vector2(move, 0) : new Vector2(0, move);

        Position += delta;
        Collider.position = Position;

        foreach (Actor actor in Actor.AllActors) // Assuming Actor.AllActors is a list of all actors in the game
        {
            if (actor.Collider.Overlaps(Collider))
            {
                Vector2 push = isHorizontal 
                    ? new Vector2(move > 0 ? Right - actor.Collider.xMin : Left - actor.Collider.xMax, 0)
                    : new Vector2(0, move > 0 ? Top - actor.Collider.yMin : Bottom - actor.Collider.yMax);

                actor.MoveX(push.x, actor.Squish);
                actor.MoveY(push.y, actor.Squish);
            }
            else if (ridingActors.Contains(actor))
            {
                if (isHorizontal)
                    actor.MoveX(move, null);
                else
                    actor.MoveY(move, null);
            }
        }
    }

    private List<Actor> GetRidingActors()
    {
        List<Actor> ridingActors = new List<Actor>();
        foreach (Actor actor in Actor.AllActors)
        {
            if (actor.IsRiding(this))
                ridingActors.Add(actor);
        }
        return ridingActors;
    }

    private float Left => Position.x;
    private float Right => Position.x + Collider.width;
    private float Top => Position.y + Collider.height;
    private float Bottom => Position.y;
}
