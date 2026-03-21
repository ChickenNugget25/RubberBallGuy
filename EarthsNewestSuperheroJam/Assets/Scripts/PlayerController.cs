using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float idleGravity = 5f;
    [SerializeField] private PhysicsMaterial2D idleMaterial;
    [SerializeField] private float poundGravity = 5f;
    [SerializeField] private PhysicsMaterial2D poundMaterial;
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Input Actions")]
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction poundAction;

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
            pounding = true;
        }
        else
        {
            if (pounding && IsGrounded()) rb.AddForce(Vector2.down * 15f, ForceMode2D.Impulse);
            if (rb != null) rb.gravityScale = idleGravity;
            if (rb != null) rb.sharedMaterial = idleMaterial;
            pounding = false;
        }
    }

    void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        if (rb != null) rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, moveInput.x * moveSpeed,0.5f);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}