using UnityEngine;

public class Civilian : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] Transform GroundCheckLeft;
    [SerializeField] Transform GroundCheckRight;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform CivilianSprite;

    bool directionRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GroundCheckLeft == null || GroundCheckRight == null) return;
        MoveCivilian();
        CheckGround();
    }

    void MoveCivilian()
    {
        if (directionRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

    void CheckGround()
    {
        if (directionRight)
        {
            Debug.DrawLine(GroundCheckRight.position, GroundCheckRight.position + Vector3.down * 0.25f, Color.red);
            if (!Physics2D.Raycast(GroundCheckRight.position, Vector2.down, 0.25f, groundLayer))
            {
                FlipDirection();
            }

            Debug.DrawLine(GroundCheckRight.position, GroundCheckRight.position + Vector3.right * 0.25f, Color.red);
            if (Physics2D.Raycast(GroundCheckRight.position, Vector2.right, 0.25f, groundLayer))
            {
                FlipDirection();
            }
        }
        else
        {
            Debug.DrawLine(GroundCheckLeft.position, GroundCheckLeft.position + Vector3.down * 0.25f, Color.red);
            if (!Physics2D.Raycast(GroundCheckLeft.position, Vector2.down, 0.25f, groundLayer))
            {
                FlipDirection();
            }

            Debug.DrawLine(GroundCheckLeft.position, GroundCheckLeft.position + Vector3.left * 0.25f, Color.red);
            if (Physics2D.Raycast(GroundCheckLeft.position, Vector2.left, 0.25f, groundLayer))
            {
                FlipDirection();
            }
        }
    }

    void FlipDirection()
    {
        directionRight = !directionRight;
        if (CivilianSprite == null) return;
        if (directionRight)
        {
            CivilianSprite.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            CivilianSprite.localScale = new Vector3(-1, 1, 1);
        }
    }
}
