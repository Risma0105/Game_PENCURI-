using UnityEngine;
using TMPro; // Wajib di-include karena kita pakai TextMeshPro
using UnityEngine.SceneManagement; // Dipakai untuk reload scene saat kalah

public class TimerGame : MonoBehaviour
{
    [Header("Component UI")]
    public TMP_Text timerText; // Tempat narik UI teks nanti

    [Header("Pengaturan Waktu (Detik)")]
    public float waktuTersisa = 120f; // Default 2 menit (bisa diganti di Inspector)
    private bool timerBerjalan = false;

    void Start()
    {
        // Untuk testing di scene ini, timer langsung kita jalankan saat Play
        timerBerjalan = true; 
    }

    void Update()
    {
        if (timerBerjalan)
        {
            if (waktuTersisa > 0)
            {
                // Mengurangi waktu berdasarkan detik riil game berjalan
                waktuTersisa -= Time.deltaTime;
                UpdateDisplayWaktu(waktuTersisa);
            }
            else
            {
                Debug.Log("Waktu habis! Pencuri tertangkap!");
                waktuTersisa = 0;
                timerBerjalan = false;
                WaktuHabisAction();
            }
        }
    }

    void UpdateDisplayWaktu(float waktuUbah)
    {
        if (waktuUbah < 0) waktuUbah = 0;

        // Rumus matematika untuk memisahkan Menit dan Detik
        float menit = Mathf.FloorToInt(waktuUbah / 60);
        float detik = Mathf.FloorToInt(waktuUbah % 60);

        // Mengubah angka menjadi format "00:00"
        timerText.text = string.Format("{0:00}:{1:00}", menit, detik);
    }

    void WaktuHabisAction()
    {
        // Saat testing, kalau waktu habis kita reload scene ini lagi biar bisa tes terus
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}