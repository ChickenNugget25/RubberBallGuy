using DG.Tweening;
using UnityEngine;
using System;

public class BossController : MonoBehaviour
{
    public static event Action onBossPound;

    [Header("References")]
    [SerializeField] private GameObject shooterObject;
    [SerializeField] private LayerMask groundLayer;

    private GameObject player;

    private enum BossState
    {
        Idle,
        Aiming,
        Swiping,
        Shooting,
        Pounding
    }

    private BossState currentState = BossState.Idle;
    private float stateTimer = 3f;
    private bool shootingRight = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Idle: UpdateIdle(); break;
            case BossState.Aiming: UpdateAiming(); break;
            case BossState.Pounding: UpdatePounding(); break;
            case BossState.Swiping: UpdateSwiping(); break;
            case BossState.Shooting: UpdateShooting(); break;
        }
    }

    // -------- STATE HANDLERS --------

    private void UpdateIdle()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer > 0) return;

        // Randomly choose next attack state (excluding Idle and Pounding)
        currentState = (BossState)UnityEngine.Random.Range(1, 4);

        switch (currentState)
        {
            case BossState.Aiming:
                stateTimer = 3f;
                break;

            case BossState.Swiping:
                StartSwipe();
                break;

            case BossState.Shooting:
                Debug.Log("Shooting");
                stateTimer = 5f;
                break;
        }
    }

    private void UpdateAiming()
    {
        stateTimer -= Time.deltaTime;

        // Smooth horizontal tracking of player
        var targetX = player.transform.position.x;
        var current = transform.position;
        transform.position = new Vector3(Mathf.MoveTowards(current.x, targetX, 0.01f), 3f, 0f);

        if (stateTimer > 0) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
        if (hit.collider != null)
        {
            StartPound(hit.point.y);
        }
        else
        {
            currentState = BossState.Aiming;
            stateTimer = 3f;
        }
    }

    private void UpdatePounding()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 100f, Color.red);
    }

    private void UpdateSwiping() { /* Swiping handled in sequence callback */ }

    private void UpdateShooting()
    {
        if (shooterObject == null) return;

        shooterObject.SetActive(true);

        // Rotate towards player
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        shooterObject.transform.eulerAngles = new Vector3(0, 0, angle);

        // Movement bounds
        stateTimer -= Time.deltaTime;
        float moveSpeed = 0.01f;

        if (shootingRight)
        {
            transform.Translate(Vector3.right * moveSpeed);
            if (transform.position.x >= 9.5f) shootingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed);
            if (transform.position.x <= -9.5f) shootingRight = true;
        }

        if (stateTimer <= 0)
            EndShooting();
    }

    // -------- STATE HELPERS --------

    private void StartSwipe()
    {
        bool swipeRight = UnityEngine.Random.value > 0.5f;
        Vector3 startPos = swipeRight ? new Vector3(16.5f, -2.5f, 0f) : new Vector3(-16.5f, -2.5f, 0f);
        Vector3 endPos = swipeRight ? new Vector3(-16.5f, -2.5f, 0f) : new Vector3(16.5f, -2.5f, 0f);

        Sequence swipeSequence = DOTween.Sequence();
        swipeSequence.Append(transform.DOMove(startPos, 1f).SetEase(Ease.InOutSine));
        swipeSequence.Append(transform.DOMove(endPos, 2.5f).SetEase(Ease.InOutSine).SetDelay(1.5f));
        swipeSequence.Append(transform.DOMove(new Vector3(0, 3, 0), 1f).SetEase(Ease.InOutSine).SetDelay(1.5f));

        swipeSequence.OnComplete(() =>
        {
            currentState = BossState.Idle;
            stateTimer = 3f;
        });
    }

    private void StartPound(float groundY)
    {
        currentState = BossState.Pounding;
        stateTimer = 1f;

        Sequence poundSequence = DOTween.Sequence();
        poundSequence.Append(transform.DOMoveY(groundY + 1f, 1.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => onBossPound?.Invoke()));
        poundSequence.Append(transform.DOMoveY(3f, 1.5f).SetEase(Ease.InQuad).SetDelay(1f));

        poundSequence.OnComplete(() =>
        {
            currentState = BossState.Idle;
            stateTimer = 5f;
        });
    }

    private void EndShooting()
    {
        shooterObject.SetActive(false);
        currentState = BossState.Idle;
        stateTimer = 3f;
    }
}
