using UnityEngine;
using TMPro;

public class Civilian : MonoBehaviour
{
    [Header("Setup Parameters")]
    [SerializeField] GameObject assignedZone;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float talkingDelay = 0.15f;

    [Header("Ground/Wall Detection")]
    [SerializeField] Transform GroundCheckLeft;
    [SerializeField] Transform GroundCheckRight;
    [SerializeField] LayerMask groundLayer;

    [Header("Other Components")]
    [SerializeField] Transform CivilianSprite;
    [SerializeField] TextMeshProUGUI dialogueText;

    int dialogueIndex = 0;
    string dialogue = string.Empty;
    bool isTalking = false;
    float dialogueTimer = 0f;
    bool directionRight = true;

    private void OnEnable()
    {
        CameraController.onCameraMovedToZone += DisplayDialogue;
    }
    private void OnDisable()
    {
        CameraController.onCameraMovedToZone -= DisplayDialogue;
    }

    private void Start()
    {
        dialogue = dialogueText.text;
    }

    // Update is called once per frame
    void Update()
    {
        if (GroundCheckLeft == null || GroundCheckRight == null) return;
        MoveCivilian();
        CheckGround();

        if (isTalking)
        {
            if (dialogueIndex < dialogue.Length && dialogueTimer>talkingDelay)
            {
                dialogueTimer = 0f;
                dialogueText.text += dialogue[dialogueIndex];
                dialogueIndex++;
            }
            dialogueTimer += Time.deltaTime;
        }
        else
        {
            dialogueText.text = string.Empty;
        }
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
            CivilianSprite.localScale = new Vector3(0.5f, 1, 1);
        }
        else
        {
            CivilianSprite.localScale = new Vector3(-0.5f, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void DisplayDialogue(GameObject zone)
    {
        dialogueText.text = string.Empty;
        if (zone == assignedZone)
        {
            isTalking = true;
            dialogueIndex = 0;
        }
        else
        {
            isTalking = false;
        }
    }
}
