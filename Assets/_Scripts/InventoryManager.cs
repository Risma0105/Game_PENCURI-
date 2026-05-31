using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Daftar Barang Curian")]
    public List<LootData> collectedItems = new List<LootData>();
    public int totalUang = 0;

    [Header("Pengaturan UI")]
    public Transform inventoryPanel;
    public GameObject iconPrefab;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void TambahKeInventory(LootData itemBaru)
    {
        collectedItems.Add(itemBaru);
        totalUang += itemBaru.harga;

        Debug.Log($"Barang masuk tas: {itemBaru.namaLukisan} | Total Uang: {totalUang}");

        MunculkanIkonDiUI(itemBaru);
    }

    void MunculkanIkonDiUI(LootData itemBaru)
    {
        if (itemBaru.visualLukisan != null)
        {
            GameObject ikonBaru = Instantiate(iconPrefab, inventoryPanel);
            Image komponenGambar = ikonBaru.GetComponent<Image>();
            komponenGambar.sprite = itemBaru.visualLukisan;
        }
        else
        {
            Debug.LogWarning($"Awas! Visual lukisan untuk {itemBaru.namaLukisan} belum diisi!");
        }
    }
}