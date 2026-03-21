using UnityEngine;
using System;

public class ZoneTrigger : MonoBehaviour
{
    public static event Action<Transform> onZoneEntered;

    [SerializeField] Transform assignedZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (assignedZone != null) onZoneEntered?.Invoke(assignedZone);
    }
}
