using UnityEngine;
using DG.Tweening;
using System;

public class CameraController : MonoBehaviour
{
    // Tells other scripts when the camera moves to a new zone
    public static event Action<GameObject> onCameraMovedToZone;

    [SerializeField] private Transform defaultZone;  // The zone the camera starts at — drag Zone1 here

    private void Start()
    {
        // Move the camera to the starting zone when the game loads
        if (defaultZone != null) MoveToZone(defaultZone);
    }

    private void OnEnable()
    {
        // When the boss ground pounds, shake the camera a lot
        BossController.onBossPound += () => CameraShake(0.5f, 1f);

        // When the player ground pounds, shake the camera a little
        PlayerController.onPlayerGroundPound += () => CameraShake(0.3f, 0.5f);

        // When the player enters a new zone, move the camera there
        ZoneTrigger.onZoneEntered += MoveToZone;
    }

    private void OnDisable()
    {
        // Stop listening to all events when this object is turned off
        BossController.onBossPound -= () => CameraShake(0.5f, 1f);
        PlayerController.onPlayerGroundPound -= () => CameraShake(0.3f, 0.5f);
        ZoneTrigger.onZoneEntered -= MoveToZone;
    }

    // Shakes the camera — duration is how long, magnitude is how hard
    void CameraShake(float duration, float magnitude)
    {
        transform.DOShakePosition(duration, magnitude);
    }

    // Smoothly slides the camera to the new zone and tells other scripts we moved
    void MoveToZone(Transform newZone)
    {
        transform.DOMove(newZone.position, 0.2f).SetEase(Ease.InOutSine);
        onCameraMovedToZone?.Invoke(newZone.gameObject);
    }
}