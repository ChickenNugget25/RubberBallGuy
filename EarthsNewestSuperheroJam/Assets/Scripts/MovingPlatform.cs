using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;          // Optional third point — leave unassigned to use A/B only
    public float speed = 2f;
    public Vector2 Velocity { get; private set; }

    private Rigidbody2D rb;
    private Vector2 target;
    private int currentTargetIndex = 1;   // Tracks which point we're heading to
    private bool reverse = false;          // Are we moving backward through the points
    private List<Vector2> points = new List<Vector2>();
    private HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();
    private Vector2 lastPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Build the list of active points
        points.Add(pointA.position);
        points.Add(pointB.position);
        if (pointC != null) points.Add(pointC.position);  // Only add C if assigned

        target = points[currentTargetIndex];
        lastPosition = rb.position;
    }

    void FixedUpdate()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Move all passengers by the same delta as the platform
        Vector2 delta = newPos - lastPosition;
        foreach (var passenger in passengers)
        {
            if (passenger != null)
            {
                passenger.position += delta;
            }
        }

        Velocity = ((Vector2)transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = newPos;

        // When we reach the target point, advance to the next one
        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            AdvanceTarget();
        }
    }

    void AdvanceTarget()
    {
        if (reverse)
        {
            currentTargetIndex--;
            if (currentTargetIndex <= 0)
            {
                currentTargetIndex = 0;
                reverse = false;  // Hit the start, switch back to forward
            }
        }
        else
        {
            currentTargetIndex++;
            if (currentTargetIndex >= points.Count - 1)
            {
                currentTargetIndex = points.Count - 1;
                reverse = true;  // Hit the end, switch to reverse
            }
        }

        target = points[currentTargetIndex];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            // Only carry objects standing on top of the platform
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    passengers.Add(collision.rigidbody);
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            passengers.Remove(collision.rigidbody);
        }
    }
}