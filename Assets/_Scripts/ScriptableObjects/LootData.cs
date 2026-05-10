using UnityEngine;

[CreateAssetMenu(fileName = "NewLoot", menuName = "Pencuri/Loot Data")]
public class LootData : ScriptableObject
{
    public string namaLukisan;
    public int harga; // Untuk skor nanti [cite: 52]
    public Sprite visualLukisan;
}