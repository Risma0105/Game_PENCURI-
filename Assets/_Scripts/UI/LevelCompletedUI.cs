using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    public void BackToLevelSelect()
    {
        int currentLevel =
            PlayerPrefs.GetInt("CurrentLevel", 1);

        int unlockedLevel =
            PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel &&
            currentLevel < 3)
        {
            PlayerPrefs.SetInt(
                "UnlockedLevel",
                currentLevel + 1);
        }

        SceneManager.LoadScene("LevelSelect");
    }
}