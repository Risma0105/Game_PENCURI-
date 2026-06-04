using UnityEngine;
using UnityEngine.SceneManagement;

public class May_MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}