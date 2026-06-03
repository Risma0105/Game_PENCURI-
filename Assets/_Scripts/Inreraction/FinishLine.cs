using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Memastikan objek yang menyentuh adalah Player
        if (collision.CompareTag("Player"))
        {
            // Langsung panggil fungsi menang yang ada di script UIManager-mu!
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.LevelCompleted();
            }
        }
    }
}