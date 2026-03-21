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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jumpBarSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpBarSlider == null) return;
        jumpBarSlider.value = Mathf.MoveTowards(jumpBarSlider.value, targetValue, Time.deltaTime * 5f);
    }

    void UpdateJumpBar(float newTarget)
    {
        targetValue = Mathf.Clamp((newTarget-10) / 14f,0f,1f);
    }
}
