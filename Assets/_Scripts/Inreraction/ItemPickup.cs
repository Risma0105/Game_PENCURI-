using UnityEngine;

public class ItemPickup : MonoBehaviour {
    // Menghubungkan ke ScriptableObject yang kamu buat
    public LootData dataLukisan;

    void OnTriggerStay2D(Collider2D other) {
        // Logika menekan tombol Space untuk mengambil barang
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space)) {
            if (dataLukisan != null) {
                Debug.Log("Berhasil mencuri: " + dataLukisan.namaLukisan);
            }
            // Barang hilang dari scene setelah diambil
            Destroy(gameObject); 
        }
    }
}