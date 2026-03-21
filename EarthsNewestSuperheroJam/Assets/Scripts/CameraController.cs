using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerController.onPlayerGroundPound += GroundPoundShake;
        ZoneTrigger.onZoneEntered += MoveToZone;
    }
    private void OnDisable()
    {
        PlayerController.onPlayerGroundPound -= GroundPoundShake;
        ZoneTrigger.onZoneEntered -= MoveToZone;
    }

    void GroundPoundShake()
    {
        CameraShake(0.3f, 0.5f);
    }

    void CameraShake(float duration, float magnitude)
    {
        transform.DOShakePosition(duration, magnitude);
    }

    void MoveToZone(Transform newZone)
    {
        transform.DOMove(newZone.position, 0.2f).SetEase(Ease.InOutSine);
    }
}
