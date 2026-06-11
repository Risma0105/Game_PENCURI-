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
    private GameObject patung;

    [SerializeField]
    private GameObject berlian;

    [SerializeField]
    private GameObject keyObject;

    [SerializeField]
    private GameObject finishDoor;

    [Header("Level Maps")]
    [SerializeField]
    private GameObject[] allMaps;

    [Header("Testing")]
    [SerializeField]
    private bool testingMode;

    [SerializeField]
    private int testingLevel = 1;

    private void Start()
    {
        int selectedLevel;

        if (testingMode)
        {
            selectedLevel = testingLevel;
        }
        else
        {
            selectedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        // Pengaman supaya tidak error
        selectedLevel = Mathf.Clamp(selectedLevel, 1, levels.Length);

        currentLevel = levels[selectedLevel - 1];

        LoadLevel(currentLevel, selectedLevel);
    }

    private void LoadLevel(LevelData levelData, int selectedLevel)
    {
        // =========================
        // MAP SYSTEM
        // =========================

        foreach (GameObject map in allMaps)
        {
            if (map != null)
            {
                map.SetActive(false);
            }
        }

        if (selectedLevel - 1 < allMaps.Length)
        {
            allMaps[selectedLevel - 1].SetActive(true);
        }

        Debug.Log("Loading Level: " + levelData.levelName);

        // =========================
        // OBJECTIVE SYSTEM
        // =========================

        lukisan.SetActive(false);
        patung.SetActive(false);
        berlian.SetActive(false);
        keyObject.SetActive(false);

        finishDoor.SetActive(true);

        switch (levelData.objectiveItem)
        {
            case "Lukisan":
                lukisan.SetActive(true);
                break;

            case "Patung":
                patung.SetActive(true);
                break;

            case "Berlian":
                berlian.SetActive(true);
                break;
        }

        // =========================
        // KEY SYSTEM
        // =========================

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