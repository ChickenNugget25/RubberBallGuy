using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float bulletImpactForce = 5f;
    public float bulletSize = 1f;
    public float bulletLifetime = 5f;

    [Header("Behavior")]
    public bool bulletsBounce = false;

    [Header("Spawning")]
    public float spawnRate = 1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / spawnRate)
        {
            SpawnBullet();
            timer = 0f;
        }
    }

    void SpawnBullet()
    {
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        Vector2 dir = transform.up; // rotation-based shooting

        bullet.Initialize(
            dir,
            bulletSpeed,
            bulletImpactForce,
            bulletSize,
            bulletsBounce,
            bulletLifetime
        );
    }
}