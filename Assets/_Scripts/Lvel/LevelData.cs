using UnityEngine;

[CreateAssetMenu(
    fileName = "NewLevelData",
    menuName = "Game/Level Data"
)]

public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName;

    [Header("Gameplay")]
    public float timer;
    public int enemyCount;

    [Header("Objective")]
    public string objectiveItem;

    [Header("Puzzle System")]
    public bool requiresKey;
    public bool requiresDoor;
}