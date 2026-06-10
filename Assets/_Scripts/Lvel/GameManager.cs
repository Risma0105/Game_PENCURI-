using UnityEngine;

public class May_GameManager : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField]
    private LevelData[] levels;

    private LevelData currentLevel;

    [Header("Level Objects")]
    [SerializeField]
    private GameObject lukisan;

    [SerializeField]
    private GameObject keyObject;

    [SerializeField]
    private GameObject finishDoor;

    [SerializeField]
private GameObject patung;

[SerializeField]
private GameObject berlian;

    void Start()
    {
        int selectedLevel =
            PlayerPrefs.GetInt("CurrentLevel", 1);

        currentLevel =
            levels[selectedLevel - 1];

        LoadLevel(currentLevel);
    }

    void LoadLevel(LevelData levelData)
{
    Debug.Log("Level Name: " +
        levelData.levelName);

    // Reset semua object
    lukisan.SetActive(false);
    patung.SetActive(false);
    berlian.SetActive(false);
    keyObject.SetActive(false);

    // Semua level punya pintu keluar
    finishDoor.SetActive(true);

    // Objective Item
    if (levelData.objectiveItem == "Lukisan")
    {
        lukisan.SetActive(true);
    }
    else if (levelData.objectiveItem == "Patung")
    {
        patung.SetActive(true);
    }
    else if (levelData.objectiveItem == "Berlian")
    {
        berlian.SetActive(true);
    }

    // Sistem kunci
    if (levelData.requiresKey)
    {
        keyObject.SetActive(true);

        Debug.Log("Door is LOCKED");
    }
    else
    {
        Debug.Log("Door is OPEN");
    }
}
}