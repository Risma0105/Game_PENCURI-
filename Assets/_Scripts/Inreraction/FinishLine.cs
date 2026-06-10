using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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