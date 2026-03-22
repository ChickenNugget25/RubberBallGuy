using UnityEngine;
using DG.Tweening;

public class BossRoomTrigger : MonoBehaviour
{
    public static event System.Action onBossRoomEntered;

    [SerializeField] private float newOrthographicSize = 8f;  // Target camera size on enter
    [SerializeField] private float transitionDuration = 0.5f; // How long the resize takes

    [Header("Spawning")]
    [SerializeField] private GameObject objectToSpawn;    // Drag your prefab here in the Inspector
    [SerializeField] private Transform spawnPoint;        // Drag an empty GameObject here to mark the spawn location

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Resize the camera
            DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, newOrthographicSize, transitionDuration);

            // Spawn the object at the spawn point if both are assigned
            if (objectToSpawn != null && spawnPoint != null)
            {
                Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            }
            onBossRoomEntered?.Invoke();

            // Destroy this trigger object so it can never fire again
            Destroy(gameObject);
        }
    }
}