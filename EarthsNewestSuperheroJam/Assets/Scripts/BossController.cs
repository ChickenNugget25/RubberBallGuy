using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class BossController : MonoBehaviour
{
    public static event Action onBossPound;

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
    int attackRounds = 3; // Number of attack rounds before becoming distracted

    // Update is called once per frame
    void Update()
    {
        switch(currentBossState)
        {
            case BossState.Idle:
                stateTimer -= Time.deltaTime;
                if(stateTimer <= 0)
                {
                    print("Aiming");
                    //Pick random attack state
                    currentBossState = (BossState)1; //Random.Range(1, 3); // Randomly choose between Aiming, Swiping, or Shooting
                    stateTimer = 3f; // Reset timer for the new state
                }
                break;
            case BossState.Aiming:
                stateTimer -= Time.deltaTime;
                transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, player.transform.position.x, 0.01f),3f,0f);
                if (stateTimer <= 0)
                {
                    print("Pounding");
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
                            if (attackRounds <= 0) { print("Distracted"); currentBossState = BossState.Idle; }
                            else
                            {
                                print("Aiming");
                                attackRounds--;
                                currentBossState = BossState.Aiming; // Transition back to Aiming for the next attack round
                            }
                            stateTimer = 5f; // Set timer for distracted state
                        });
                    }
                }
                break;
            case BossState.Pounding:
                Debug.DrawLine(transform.position, transform.position + Vector3.down * 100, Color.red);
                break;
            case BossState.Swiping:
                // Swipe attack behavior
                break;
            case BossState.Shooting:
                // Ranged attack behavior
                break;
        }
    }
}
