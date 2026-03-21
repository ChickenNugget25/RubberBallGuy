using UnityEngine;
using UnityEngine.UI;

public class JumpBar : MonoBehaviour
{
    Slider jumpBarSlider;
    float targetValue = 0f;
    PlayerController playerController;

    private void OnEnable()
    {
        PlayerController.onJumpForceChanged += UpdateJumpBar;
    }

    private void OnDisable()
    {
        PlayerController.onJumpForceChanged -= UpdateJumpBar;
    }

    void Start()
    {
        jumpBarSlider = GetComponent<Slider>();
        // Grab the PlayerController so we can read maxJumpForce
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (jumpBarSlider == null) return;
        jumpBarSlider.value = Mathf.MoveTowards(jumpBarSlider.value, targetValue, Time.deltaTime * 5f);
    }

    void UpdateJumpBar(float newTarget)
    {
        float min = 10f;  // Default jump force (the baseline, bar shows 0 here)
        float max = playerController != null ? playerController.maxJumpForce : 24f;  // Fallback to 24 if not found
        targetValue = Mathf.Clamp((newTarget - min) / (max - min), 0f, 1f);
        Debug.Log(targetValue);
    }
}