using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    public static event Action onPlayerGroundPound;
    public static event Action<float> onJumpForceChanged;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float idleGravity = 5f;
    [SerializeField] private PhysicsMaterial2D idleMaterial;
    [SerializeField] private float poundGravity = 5f;
    [SerializeField] private PhysicsMaterial2D poundMaterial;
    [Header("Ground Detection")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Input Actions")]
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction poundAction;

    float jumpForce = 0f;
    float chargeTimer = 0f;
    bool grounded = false;
    bool pounding = false;
    Rigidbody2D rb;


    private UnityEngine.Vector2 externalForce = Vector2.zero;
    private UnityEngine.Vector2 playerMovementForce = Vector2.zero;
    public void AddExternalForce(UnityEngine.Vector2 force)
    {
        externalForce.x += force.x;
        externalForce.y += force.y/30; // scale down vertical force to prevent excessive launch height
        externalForce.x = Mathf.Clamp(externalForce.x, -25f, 25f); // clamp horizontal force to prevent excessive speed
        externalForce.y = Mathf.Clamp(externalForce.y, -25f, 25f); // clamp vertical force to prevent excessive launch height
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        externalForce.x = Mathf.Lerp(externalForce.x, 0f, 5.0f * Time.deltaTime); // Gradually reduce external force over time
        externalForce.y = Mathf.Lerp(externalForce.y, 0f, 50.0f * Time.deltaTime); // Gradually reduce external force over time

        Move();
        if (poundAction.ReadValue<float>() > 0.5f)
        {
            if (rb != null) rb.gravityScale = poundGravity;
            if (rb != null) rb.sharedMaterial = poundMaterial;
            if (IsGrounded())
            {
                print("grounded");
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= 0.25f)
                {
                    chargeTimer = 0f;
                    if (jumpForce<24) setJumpForce(jumpForce + 2f);
                }
                if (!grounded) onPlayerGroundPound?.Invoke();
            }
            pounding = true;
        }
        else
        {
            chargeTimer = 0f;
            if (pounding && grounded) rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
            setJumpForce(10f);
            if (rb != null) rb.gravityScale = idleGravity;
            if (rb != null) rb.sharedMaterial = idleMaterial;
            pounding = false;
        }
        grounded = IsGrounded();
    }

    void setJumpForce(float newJumpForce)
    {
        jumpForce = newJumpForce;
        onJumpForceChanged?.Invoke(jumpForce);
    }

    void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        //if (rb != null) rb.AddForce(new Vector2(moveInput.x * moveSpeed, 0), ForceMode2D.Force);
        if (rb != null) playerMovementForce.x = Mathf.Lerp(rb.linearVelocityX, moveInput.x * moveSpeed,0.5f);


        float playerYMovementForce = Mathf.Clamp(rb.linearVelocityY + externalForce.y, -70f, 70f);

        rb.linearVelocity = new Vector2(playerMovementForce.x + externalForce.x, playerYMovementForce);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position - new Vector3(0,0.5f,0), groundCheckRadius, groundLayer);
    }
}