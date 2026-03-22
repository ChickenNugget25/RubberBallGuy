using UnityEngine;
using TMPro;
public class Civilian : MonoBehaviour
{
    [Header("Setup Parameters")]
    [SerializeField] GameObject assignedZone;         // The zone this civilian belongs to
    [SerializeField] private float speed = 2f;        // How fast the civilian walks
    [SerializeField] private float talkingDelay = 0.15f; // Time between each character appearing in dialogue
    [SerializeField] private float spriteScale = 1f;  // Size of the civilian sprite
    [Header("Ground/Wall Detection")]
    [SerializeField] Transform GroundCheckLeft;       // Left foot raycast point
    [SerializeField] Transform GroundCheckRight;      // Right foot raycast point
    [SerializeField] LayerMask groundLayer;           // What counts as ground/wall
    [Header("Other Components")]
    [SerializeField] Transform CivilianSprite;
    [SerializeField] Sprite[] civilianSprites;
    [SerializeField] TextMeshProUGUI dialogueText;

    int dialogueIndex = 0;              // Which character of the dialogue we're up to
    string dialogue = string.Empty;     // The full dialogue string stored at start
    bool isTalking = false;             // Is this civilian currently showing dialogue
    float dialogueTimer = 0f;          // Tracks time between each character appearing
    bool directionRight = true;         // Which way the civilian is currently walking

    private void OnEnable()
    {
        // Listen for when the camera moves to a new zone
        CameraController.onCameraMovedToZone += DisplayDialogue;
    }
    private void OnDisable()
    {
        // Stop listening when this object is turned off
        CameraController.onCameraMovedToZone -= DisplayDialogue;
    }
    private void Start()
    {
        CivilianSprite.gameObject.GetComponent<SpriteRenderer>().sprite = civilianSprites[Random.Range(0, civilianSprites.Length)];
        dialogue = dialogueText.text;
    }
    void Update()
    {
        if (GroundCheckLeft == null || GroundCheckRight == null) return;

        MoveCivilian();
        CheckGround();

        if (isTalking)
        {
            // Add one letter at a time after each talkingDelay interval
            if (dialogueIndex < dialogue.Length && dialogueTimer > talkingDelay)
            {
                dialogueTimer = 0f;
                dialogueText.text += dialogue[dialogueIndex];
                dialogueIndex++;
            }
            dialogueTimer += Time.deltaTime;
        }
        else
        {
            // Clear the text box when not talking
            dialogueText.text = string.Empty;
        }
    }

    void MoveCivilian()
    {
        // Walk left or right depending on current direction
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
            // If there's no ground ahead on the right, turn around
            Debug.DrawLine(GroundCheckRight.position, GroundCheckRight.position + Vector3.down * 0.25f, Color.red);
            if (!Physics2D.Raycast(GroundCheckRight.position, Vector2.down, 0.25f, groundLayer))
            {
                FlipDirection();
            }
            // If there's a wall ahead on the right, turn around
            Debug.DrawLine(GroundCheckRight.position, GroundCheckRight.position + Vector3.right * 0.25f, Color.red);
            if (Physics2D.Raycast(GroundCheckRight.position, Vector2.right, 0.25f, groundLayer))
            {
                FlipDirection();
            }
        }
        else
        {
            // If there's no ground ahead on the left, turn around
            Debug.DrawLine(GroundCheckLeft.position, GroundCheckLeft.position + Vector3.down * 0.25f, Color.red);
            if (!Physics2D.Raycast(GroundCheckLeft.position, Vector2.down, 0.25f, groundLayer))
            {
                FlipDirection();
            }
            // If there's a wall ahead on the left, turn around
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
            CivilianSprite.localScale = new Vector3(0.14f, 0.14f, 0.14f);
        }
        else
        {
            CivilianSprite.localScale = new Vector3(-0.14f, 0.14f, 0.14f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Remove the civilian when the player reaches them (rescued!)
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
            // Start talking when the camera moves to this civilian's zone
            isTalking = true;
            dialogueIndex = 0;
        }
        else
        {
            // Stop talking when the camera moves away
            isTalking = false;
        }
    }
}