using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController Player; // The target the camera will follow

    private void OnEnable()
    {
        PlayerController.onPlayerGroundPound += GroundPoundShake;
    }
    private void OnDisable()
    {
        PlayerController.onPlayerGroundPound -= GroundPoundShake;
    }

    void GroundPoundShake()
    {
        CameraShake(0.3f, 0.5f);
    }

    void CameraShake(float duration, float magnitude)
    {
        transform.DOShakePosition(duration, magnitude);
    }
}
