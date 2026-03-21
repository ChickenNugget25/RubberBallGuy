using UnityEngine;
using UnityEngine.UI;

public class JumpBar : MonoBehaviour
{
    Slider jumpBarSlider;
    float targetValue = 0f;

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
    }

    void Update()
    {
        if (jumpBarSlider == null) return;
        jumpBarSlider.value = Mathf.MoveTowards(jumpBarSlider.value, targetValue, Time.deltaTime * 5f);
    }

    void UpdateJumpBar(float newTarget)
    {
        float min = 10f;  // Default jump force (the baseline, bar shows 0 here)
        float max = PlayerController.StaticMaxJumpForce;  // Max jump force (the cap, bar shows 1 here)
        targetValue = Mathf.Clamp((newTarget - min) / (max - min), 0f, 1f);
    }
}