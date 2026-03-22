using UnityEngine;

public class SpawnerActivator : MonoBehaviour
{
    [SerializeField] private float activationRange = 20f;  // Distance from player to activate spawners

    private BulletSpawner[] allSpawners;

    void Start()
    {
        // Grab all spawners in the scene once at start
        allSpawners = FindObjectsByType<BulletSpawner>(FindObjectsSortMode.None);
    }

    void Update()
    {
        foreach (BulletSpawner spawner in allSpawners)
        {
            if (spawner == null) continue;

            float distance = Vector2.Distance(transform.position, spawner.transform.position);
            spawner.enabled = distance <= activationRange;
        }
    }

    // Draws the activation range as a circle in the editor so you can see it visually
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}