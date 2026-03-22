using System;
using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{
    public static event Action onBossHurt;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BossWeakPoint.onBossHurt?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
