using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI timerText;
    int minuteValue = 0;
    int secondValue = 0;
    float secondTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        secondTimer += Time.deltaTime;
        if (secondTimer >= 1f)
        {
            secondTimer = 0f;
            secondValue++;
            if (secondValue >= 60)
            {
                secondValue = 0;
                minuteValue++;
            }
            timerText.text = $"{minuteValue:00}:{secondValue:00}";
        }

    }
}
