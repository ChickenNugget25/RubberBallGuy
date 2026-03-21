using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
        print("ran");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
        print("credits");
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenuScene");
        print("back");
    }
}
