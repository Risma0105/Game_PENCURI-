using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class May_LevelSelect : MonoBehaviour
{
    [SerializeField] private Button btnMap2;
    [SerializeField] private Button btnMap3;

    private void Start()
    {
      
        int unlockedLevel =
            PlayerPrefs.GetInt("UnlockedLevel", 1);

        btnMap2.interactable =
            unlockedLevel >= 2;

        btnMap3.interactable =
            unlockedLevel >= 3;
    }

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

    public void ResetProgress()
{
    PlayerPrefs.DeleteAll();

    btnMap2.interactable = false;
    btnMap3.interactable = false;

    Debug.Log("Progress berhasil direset!");
}
}