using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("OfficeLevels"); // Make sure your game scene is named correctly
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting..."); // Works only in a built game
    }
}

