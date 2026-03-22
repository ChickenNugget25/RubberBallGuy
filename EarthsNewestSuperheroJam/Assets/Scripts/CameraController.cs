using UnityEngine;
using DG.Tweening;
using System;

public class CameraController : MonoBehaviour
{
    public static event Action<GameObject> onCameraMovedToZone;

    private void OnEnable()
    {
        BossController.onBossPound += () => CameraShake(0.5f, 1f);
        PlayerController.onPlayerGroundPound += () => CameraShake(0.3f, 0.5f);
        ZoneTrigger.onZoneEntered += MoveToZone;
    }
    private void OnDisable()
    {
        BossController.onBossPound -= () => CameraShake(0.5f, 1f);
        PlayerController.onPlayerGroundPound -= () => CameraShake(0.3f, 0.5f);
        ZoneTrigger.onZoneEntered -= MoveToZone;
    }

    void CameraShake(float duration, float magnitude)
    {
        transform.DOShakePosition(duration, magnitude);
    }

    void MoveToZone(Transform newZone)
    {
        transform.DOMove(newZone.position, 0.2f).SetEase(Ease.InOutSine);
        onCameraMovedToZone?.Invoke(newZone.gameObject);
    }
}
