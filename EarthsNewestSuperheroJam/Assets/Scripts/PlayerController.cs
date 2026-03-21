using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [Header("Input Actions")]
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction poundAction;

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
    }

    void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Debug.Log("Move Input: " + moveInput);
        if (rb != null) rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, moveInput.x * moveSpeed,0.2f);
    }
}