using UnityEngine;

public class May_GameManager : MonoBehaviour
{
    void Start()
    {
        int level =
            PlayerPrefs.GetInt("CurrentLevel", 1);

        Debug.Log("Current Level: " + level);

        switch(level)
        {
            case 1:
                SetupLevel1();
                break;

            case 2:
                SetupLevel2();
                break;

            case 3:
                SetupLevel3();
                break;
        }
    }

    void SetupLevel1()
    {
        Debug.Log("Level 1 Loaded");
    }

    void SetupLevel2()
    {
        Debug.Log("Level 2 Loaded");
    }

    void SetupLevel3()
    {
        Debug.Log("Level 3 Loaded");
    }
}