using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public delegate void OnZoneEntered(Transform newZone);
    public static event OnZoneEntered onZoneEntered;

    [SerializeField] Transform assignedZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (assignedZone != null) onZoneEntered?.Invoke(assignedZone);
    }
}
