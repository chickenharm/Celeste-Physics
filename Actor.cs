using System;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector2 Position { get; private set; }
    private float xRemainder;
    private float yRemainder;
    public Rect Collider; // Rect representing the bounding box of the Actor

    public void Initialize(Vector2 initialPosition, Vector2 colliderSize)
    {
        Position = initialPosition;
        Collider = new Rect(initialPosition, colliderSize);
    }

    public void MoveX(float amount, Action onCollide)
    {
        xRemainder += amount;
        int move = Mathf.RoundToInt(xRemainder);

        if (move != 0)
        {
            xRemainder -= move;
            int direction = Math.Sign(move);

            while (move != 0)
            {
                if (!CollideAt(Position + new Vector2(direction, 0)))
                {
                    Position += new Vector2(direction, 0);
                    Collider.position = Position;
                    move -= direction;
                }
                else
                {
                    onCollide?.Invoke();
                    break;
                }
            }
        }
    }

    public void MoveY(float amount, Action onCollide)
    {
        yRemainder += amount;
        int move = Mathf.RoundToInt(yRemainder);

        if (move != 0)
        {
            yRemainder -= move;
            int direction = Math.Sign(move);

            while (move != 0)
            {
                if (!CollideAt(Position + new Vector2(0, direction)))
                {
                    Position += new Vector2(0, direction);
                    Collider.position = Position;
                    move -= direction;
                }
                else
                {
                    onCollide?.Invoke();
                    break;
                }
            }
        }
    }

    protected virtual bool CollideAt(Vector2 position)
    {
        // Check collision with solids
        foreach (Solid solid in Solid.AllSolids)
        {
            if (solid.Collider.Overlaps(new Rect(position, Collider.size)))
                return true;
        }
        return false;
    }

    public virtual bool IsRiding(Solid solid)
    {
        return Collider.Overlaps(solid.Collider) && Position.y >= solid.Position.y;
    }

    public virtual void Squish()
    {
        // Default behavior: destroy the actor
        Destroy(gameObject);
    }
}
