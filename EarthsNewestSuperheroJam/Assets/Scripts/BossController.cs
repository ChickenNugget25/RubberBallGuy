using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class BossController : MonoBehaviour
{
    public static event Action onBossPound;

    [SerializeField] GameObject shooterObject;
    [SerializeField] LayerMask groundLayer;

    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    enum BossState
    {
        Idle,
        Aiming,
        Swiping,
        Shooting,
        Pounding,
    }
    BossState currentBossState = BossState.Idle;
    float stateTimer = 3f;
    bool shootingRight = true;

    // Update is called once per frame
    void Update()
    {
        switch(currentBossState)
        {
            case BossState.Idle:
                stateTimer -= Time.deltaTime;
                if(stateTimer <= 0)
                {
                    //Pick random attack state
                    currentBossState = (BossState)UnityEngine.Random.Range(1, 4); // Randomly choose between Aiming, Swiping, or Shooting
                    if (currentBossState == BossState.Aiming) stateTimer = 3f;
                    else if (currentBossState == BossState.Swiping)
                    {
                        // Choose random direction to swipe
                        bool swipeRight = UnityEngine.Random.value > 0.5f;
                        Vector3 startPosition = swipeRight ? new Vector3(16.5f, -2.5f, 0f) : new Vector3(-16.5f, -2.5f, 0f);
                        Vector3 endPosition = swipeRight ? new Vector3(-16.5f, -2.5f, 0f) : new Vector3(16.5f, -2.5f, 0f);
                        DG.Tweening.Sequence swipeSequence = DOTween.Sequence();
                        swipeSequence.Append(transform.DOMove(startPosition, 1f).SetEase(Ease.InOutSine));
                        swipeSequence.Append(transform.DOMove(endPosition, 2.5f).SetEase(Ease.InOutSine).SetDelay(1.5f));
                        swipeSequence.Append(transform.DOMove(new Vector3(0,3,0), 1f).SetEase(Ease.InOutSine).SetDelay(1.5f));
                        swipeSequence.OnComplete(() =>
                        {
                            currentBossState = BossState.Idle; // Transition back to Idle after swiping
                            stateTimer = 3f; // Set timer for idle state
                        });

                    }
                    else if (currentBossState == BossState.Shooting) print("Shooting"); stateTimer = 5f; // Set timer for shooting state
                }
                break;
            case BossState.Aiming:
                stateTimer -= Time.deltaTime;
                transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, player.transform.position.x, 0.01f),3f,0f);
                if (stateTimer <= 0)
                {
                    currentBossState = BossState.Pounding; // Transition to Pounding after aiming
                    stateTimer = 1f;

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
                    if (hit.collider != null)
                    {
                        DG.Tweening.Sequence poundSequence = DOTween.Sequence();
                        poundSequence.Append(transform.DOMoveY(hit.point.y + 1f, 1.5f).SetEase(Ease.InBack).OnComplete(() =>
                        {
                            onBossPound?.Invoke();
                        }));
                        poundSequence.Append(transform.DOMoveY(3, 1.5f).SetEase(Ease.InQuad).SetDelay(1f));
                        poundSequence.OnComplete(() =>
                        {
                            // After pounding, transition to Distracted state
                            currentBossState = BossState.Idle;
                            stateTimer = 5f; // Set timer for distracted state
                        });
                    }
                    else
                    {
                        currentBossState = BossState.Aiming; // Transition back to Aiming for the next attack round
                    }
                }
                break;
            case BossState.Pounding:
                Debug.DrawLine(transform.position, transform.position + Vector3.down * 100, Color.red);
                break;
            case BossState.Swiping:
                // Swipe
                break;
            case BossState.Shooting:
                if(shooterObject != null) shooterObject.SetActive(true);
                Vector2 dir = player.transform.position - transform.position;
                shooterObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg-90f);
                stateTimer -= Time.deltaTime;
                if (shootingRight)
                {
                    transform.Translate(Vector3.right * 0.01f); // Move right while shooting
                    if (transform.position.x >= 9.5) { shootingRight = false; }
                }
                else
                {
                    transform.Translate(Vector3.left * 0.01f); // Move left while shooting
                    if (transform.position.x <= -9.5) { shootingRight = true; }
                }

                if (stateTimer <= 0)
                {
                    shooterObject.SetActive(false);
                    currentBossState = BossState.Idle; // Transition back to Idle after shooting
                    stateTimer = 3f; // Set timer for idle state
                }
                break;
        }
    }
}
