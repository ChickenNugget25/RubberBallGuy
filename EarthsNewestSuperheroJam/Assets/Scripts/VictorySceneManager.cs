using UnityEngine;

public class VictorySceneManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        UnityEngine.Debug.Log("Back to Main Menu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}
