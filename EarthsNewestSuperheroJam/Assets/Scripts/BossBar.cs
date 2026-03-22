using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossBar : MonoBehaviour
{
    [SerializeField] Slider bossBarSlider;
    float targetValue = 1f;

    private void OnEnable()
    {
        BossRoomTrigger.onBossRoomEntered += () => transform.DOMoveY(580, 1f);
        BossController.onHealthChanged += UpdateBossBar;
    }
    private void OnDisable()
    {
        BossRoomTrigger.onBossRoomEntered -= () => transform.DOMoveY(580, 1f);
        BossController.onHealthChanged -= UpdateBossBar;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        if (bossBarSlider == null) return;
        bossBarSlider.value = Mathf.Lerp(bossBarSlider.value, targetValue, 0.1f);
    }

    void UpdateBossBar(int currentHealth, int maxHealth)
    {
        targetValue = Mathf.Clamp(((float)currentHealth / (float)maxHealth), 0f, 1f);
    }
}
