using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    public LootData dataLukisan;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (dataLukisan != null)
            {
                InventoryManager.Instance.TambahKeInventory(dataLukisan);

                Debug.Log("Berhasil mencuri: " +
                          dataLukisan.namaLukisan);
            }

            GameState.objectiveCollected = true;

            Debug.Log("Objective berhasil dicuri!");

            Destroy(gameObject);
        }
    }
}