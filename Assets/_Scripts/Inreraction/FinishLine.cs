using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Memastikan objek yang menyentuh adalah Player
        if (collision.CompareTag("Player"))
        {
            // 1. Matikan visual Sprite Renderer si maling biar dia menghilang
            SpriteRenderer playerSprite = collision.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.enabled = false;
            }

            // 2. Matikan Collider si maling biar gak dideteksi raycast senter hansip lagi
            Collider2D playerCollider = collision.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }

            // 3. Panggil fungsi menang yang ada di script UIManager-mu!
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.LevelCompleted();
            }
        }
    }
}