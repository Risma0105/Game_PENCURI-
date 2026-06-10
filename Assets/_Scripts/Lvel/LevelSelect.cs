using UnityEngine;
using UnityEngine.SceneManagement;

public class May_LevelSelect : MonoBehaviour
{
    public void Level1()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        SceneManager.LoadScene("GamePlay");
    }

    public void Level2()
    {
        PlayerPrefs.SetInt("CurrentLevel", 2);
        SceneManager.LoadScene("GamePlay");
    }

    public void Level3()
    {
        PlayerPrefs.SetInt("CurrentLevel", 3);
        SceneManager.LoadScene("GamePlay");
    }
}