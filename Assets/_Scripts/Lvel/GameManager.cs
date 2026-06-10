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
    Debug.Log(
        "Objective: " +
        levelData.objectiveItem
    );

    // Reset object
    lukisan.SetActive(false);
    keyObject.SetActive(false);

    finishDoor.SetActive(true);

    if(levelData.objectiveItem
        == "Lukisan")
    {
        lukisan.SetActive(true);
    }

    if(levelData.requiresKey)
    {
        keyObject.SetActive(true);

        Debug.Log(
            "Door is LOCKED"
        );
    }
}
}