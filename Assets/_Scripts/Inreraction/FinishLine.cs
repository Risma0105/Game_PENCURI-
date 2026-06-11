+using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        int currentLevel =
            PlayerPrefs.GetInt("CurrentLevel", 1);

        // Level 3 butuh objective + kunci
        if (currentLevel == 3)
        {
            if (GameState.objectiveCollected &&
                GameState.keyCollected)
            {
                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.LevelCompleted();
                }
            }
            else
            {
                Debug.Log("Butuh objective dan kunci!");
            }
        }

        // Level 1 & 2 cukup objective
        else
        {
            if (GameState.objectiveCollected)
            {
                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.LevelCompleted();
                }
            }
            else
            {
                Debug.Log("Ambil objective dulu!");
            }
        }
    }
}
}