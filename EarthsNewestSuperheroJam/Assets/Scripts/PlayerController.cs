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
    [SerializeField] private float maxJumpForce = 24f;            // Max force applied on ground pound release

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckRadius = 0.2f;      // Size of the ground check circle
    [SerializeField] private LayerMask groundLayer;               // Which layers count as ground

    [Header("Input Actions")]
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction poundAction;

    float jumpForce = 0f;       // Current charge amount, applied on pound release
    float chargeTimer = 0f;     // Tracks time held to increment jump force
    bool grounded = false;      // Was the player grounded last frame
    bool pounding = false;      // Is the player currently holding the pound button
    Rigidbody2D rb;

    private UnityEngine.Vector2 externalForce = Vector2.zero;        // Forces added from outside (e.g. explosions)
    private UnityEngine.Vector2 playerMovementForce = Vector2.zero;  // Force from player input

    // Called by other scripts to push the player (e.g. from an explosion)
    public void AddExternalForce(UnityEngine.Vector2 force)
    {
        externalForce.x += force.x;
        externalForce.y += force.y / 30;  // Scale down vertical so the player doesn't fly too high
        externalForce.x = Mathf.Clamp(externalForce.x, -25f, 25f);  // Cap horizontal speed
        externalForce.y = Mathf.Clamp(externalForce.y, -25f, 25f);  // Cap vertical speed
    }

    void Start()
    {
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
        // Gradually bleed off external forces each frame
        externalForce.x = Mathf.Lerp(externalForce.x, 0f, 5.0f * Time.deltaTime);
        externalForce.y = Mathf.Lerp(externalForce.y, 0f, 15.0f * Time.deltaTime);

        Move();

        if (poundAction.ReadValue<float>() > 0.5f)
        {
            // Pound button held � increase gravity and switch to pound material
            if (rb != null) rb.gravityScale = poundGravity;
            if (rb != null) rb.sharedMaterial = poundMaterial;

            if (IsGrounded())
            {
                print("grounded");

                // Every 0.25s on the ground, increase jump force up to the max
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= 0.25f)
                {
                    chargeTimer = 0f;
                    if (jumpForce < maxJumpForce) setJumpForce(jumpForce + 2f);
                }

                // Fire ground pound event the first frame we hit the ground
                if (!grounded) onPlayerGroundPound?.Invoke();
            }
            pounding = true;
        }
        else
        {
            // Pound button released
            chargeTimer = 0f;

            // If we were pounding and just landed, launch the player downward
            if (pounding && grounded) rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);

            setJumpForce(10f);  // Reset jump force to default
            if (rb != null) rb.gravityScale = idleGravity;
            if (rb != null) rb.sharedMaterial = idleMaterial;
            pounding = false;
        }

        // Store grounded state for next frame comparison
        grounded = IsGrounded();
    }

    // Updates jump force and notifies any listeners
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

        // Add external vertical force but cap it to prevent extreme speeds
        float playerYMovementForce = Mathf.Clamp(rb.linearVelocityY + externalForce.y, -70f, 70f);

        // Apply final velocity combining player input and any external forces
        rb.linearVelocity = new Vector2(playerMovementForce.x + externalForce.x, playerYMovementForce);
    }

    // Returns true if a small circle at the player's feet overlaps the ground layer
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.5f, 0), groundCheckRadius, groundLayer);
    }
}