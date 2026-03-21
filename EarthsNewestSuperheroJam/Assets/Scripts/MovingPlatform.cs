using UnityEngine;
using System.Collections.Generic;


public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    public Vector2 Velocity { get; private set; }

    private Rigidbody2D rb;
    private Vector2 target;
    private HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();

    private Vector2 lastPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = pointB.position;
        lastPosition = rb.position;
    }

    void FixedUpdate()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

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

        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            target = target == (Vector2)pointA.position ? (Vector2)pointB.position : (Vector2)pointA.position;
        }
    }
        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            // Check if object is on top
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f) // standing on platform
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