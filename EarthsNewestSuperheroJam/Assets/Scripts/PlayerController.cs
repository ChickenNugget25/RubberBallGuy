using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    // Events other scripts can listen to
    public static event Action onPlayerGroundPound;
    public static event Action<float> onJumpForceChanged;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float idleGravity = 5f;
    [SerializeField] private PhysicsMaterial2D idleMaterial;      // Low friction when moving normally
    [SerializeField] private float poundGravity = 5f;
    [SerializeField] private PhysicsMaterial2D poundMaterial;     // High friction when ground pounding
    [SerializeField] public float maxJumpForce = 24f;             // Max force the jump can charge up to
    [SerializeField] private float chargeInterval = 0.25f;        // Seconds between each charge tick (lower = charges faster)
    [SerializeField] private float chargeIncrement = 2f;          // Force added per tick (higher = charges faster)

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckRadius = 0.2f;      // Size of the ground check circle
    [SerializeField] private LayerMask groundLayer;               // Which layers count as ground

    [Header("Input Actions")]
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction poundAction;

    public static float StaticMaxJumpForce;
    float jumpForce = 0f;       // Current charge amount, applied as downward force on pound release
    float chargeTimer = 0f;     // Accumulates time to trigger the next charge tick
    bool grounded = false;      // Was the player grounded last frame (used for one-frame event detection)
    bool pounding = false;      // Is the player currently holding the pound button
    Rigidbody2D rb;

    private MovingPlatform currentPlatform;                          // Platform the player is currently standing on

    private UnityEngine.Vector2 externalForce = Vector2.zero;        // Forces added from outside (e.g. explosions)
    private UnityEngine.Vector2 playerMovementForce = Vector2.zero;  // Horizontal force calculated from player input

    // Called by other scripts to push the player (e.g. from an explosion)
    public void AddExternalForce(UnityEngine.Vector2 force)
    {
        externalForce.x += force.x;
        externalForce.y += force.y / 30;  // Scale down vertical so the player doesn't fly too high
        externalForce.x = Mathf.Clamp(externalForce.x, -25f, 25f);  // Cap horizontal to prevent excessive speed
        externalForce.y = Mathf.Clamp(externalForce.y, -25f, 25f);  // Cap vertical to prevent excessive launch height
    }

    void Start()
    {
        StaticMaxJumpForce = maxJumpForce;  // Set static reference for UI and other scripts
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        poundAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        poundAction.Disable();
    }

    void Update()
    {
        // Gradually bleed off external forces each frame so they don't last forever
        externalForce.x = Mathf.Lerp(externalForce.x, 0f, 5.0f * Time.deltaTime);
        externalForce.y = Mathf.Lerp(externalForce.y, 0f, 15.0f * Time.deltaTime);

        Move();

        if (poundAction.ReadValue<float>() > 0.5f)
        {
            // Pound button held — increase gravity and switch to high-friction material
            if (rb != null) rb.gravityScale = poundGravity;
            if (rb != null) rb.sharedMaterial = poundMaterial;

            if (IsGrounded())
            {
                print("grounded");

                // Every chargeInterval seconds, increase jump force by chargeIncrement up to maxJumpForce
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= chargeInterval)
                {
                    chargeTimer = 0f;
                    if (jumpForce < maxJumpForce) setJumpForce(jumpForce + chargeIncrement);
                }

                // Fire ground pound event only on the first frame we touch the ground
                if (!grounded) onPlayerGroundPound?.Invoke();
            }
            pounding = true;
        }
        else
        {
            // Pound button released — reset charge timer
            chargeTimer = 0f;

            // If we were pounding and are grounded, launch the player downward with charged force
            if (pounding && grounded) rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);

            setJumpForce(10f);  // Reset jump force to minimum default
            if (rb != null) rb.gravityScale = idleGravity;
            if (rb != null) rb.sharedMaterial = idleMaterial;
            pounding = false;
        }

        // Store grounded state at end of frame for next frame comparison
        grounded = IsGrounded();
    }

    // Updates jump force and notifies any listeners (e.g. the jump bar UI)
    void setJumpForce(float newJumpForce)
    {
        jumpForce = newJumpForce;
        onJumpForceChanged?.Invoke(jumpForce);
    }

    void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Smoothly slide horizontal velocity toward the target speed
        if (rb != null) playerMovementForce.x = Mathf.Lerp(rb.linearVelocityX, moveInput.x * moveSpeed, 0.5f);

        // Combine current vertical velocity with external force, capped to prevent extreme speeds
        float playerYMovementForce = Mathf.Clamp(rb.linearVelocityY + externalForce.y, -70f, 70f);

        // Apply final velocity: player input + external forces + any moving platform we're standing on
        Vector2 platformVelocity = currentPlatform != null ? currentPlatform.Velocity : Vector2.zero;
        rb.linearVelocity = new Vector2(playerMovementForce.x + externalForce.x, playerYMovementForce) + platformVelocity;
    }

    // Returns true if a small circle at the player's feet overlaps the ground layer
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.5f, 0), groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MovingPlatform platform = collision.collider.GetComponent<MovingPlatform>();

        if (platform != null)
        {
            // Only count as on the platform if we're landing on top of it
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    currentPlatform = platform;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Clear the platform reference when we leave it
        MovingPlatform platform = collision.collider.GetComponent<MovingPlatform>();

        if (platform != null && platform == currentPlatform)
        {
            currentPlatform = null;
        }
    }
}