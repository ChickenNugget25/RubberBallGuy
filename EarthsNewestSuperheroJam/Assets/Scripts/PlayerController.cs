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
        if (rb != null) rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, moveInput.x * moveSpeed,0.5f);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position - new Vector3(0,0.5f,0), groundCheckRadius, groundLayer);
    }
}