using UnityEngine;

public class ForceReceiver : MonoBehaviour
{
    private Rigidbody2D rb;

    // Optional: link to player controller
    private PlayerController player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerController>();
    }

    public void ApplyForce(Vector2 force, ForceMode2D mode)
    {
        // If this object has a player controller, redirect force
        if (player != null)
        {
            //player.AddExternalForce(force);
        }
        else
        {
            rb.AddForce(force, mode);
        }
    }
}