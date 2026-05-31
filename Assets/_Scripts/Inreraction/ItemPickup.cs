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
                // Masukkan barang ke Inventory
                InventoryManager.Instance.TambahKeInventory(dataLukisan);
                Debug.Log("Berhasil mencuri: " + dataLukisan.namaLukisan);
            }

            // Hancurkan barang dari scene
            Destroy(gameObject);
        }
    }
}