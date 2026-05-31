using UnityEngine;
using UnityEngine.InputSystem; // WAJIB ditambahkan untuk memanggil New Input System

public class ItemPickup : MonoBehaviour
{
    public LootData dataLukisan;

    void OnTriggerStay2D(Collider2D other)
    {
        // Mengecek apakah yang menyentuh adalah "Player" 
        // DAN mengecek apakah tombol Space ditekan menggunakan New Input System
        if (other.CompareTag("Player") && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (dataLukisan != null)
            {
                // 1. Masukkan barang ke Inventory Humayra
                InventoryManager.Instance.TambahKeInventory(dataLukisan);
                Debug.Log("Berhasil mencuri: " + dataLukisan.namaLukisan);
            }

            // 2. KODE GABUNGAN: Panggil UIManager milik Risma untuk memunculkan panel bintang
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.LevelCompleted();
            }

            // 3. Hancurkan barang dari scene
            Destroy(gameObject);
        }
    }
}