using UnityEngine;

public class BonusLevelFinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("VictoryScene");
    }
}
