using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    [Header("Star Scoring Settings (In Seconds)")]
    // 1-3 Menit logika bintang:
    // Bintang 3: < 1 menit (60 detik)
    // Bintang 2: 1 - 2 menit (60 - 120 detik)
    // Bintang 1: 2 - 3 menit (120 - 180 detik)
    [SerializeField] private float threeStarsLimit = 60f;
    [SerializeField] private float twoStarsLimit = 120f;
    [SerializeField] private float maxTimeLimit = 180f; // 3 Menit maks

    [Header("Alert Settings")]
    [SerializeField] private Image alertOverlay;
    [SerializeField] private float pulseSpeed = 2f;
    private bool isAlertActive = false;
    private Coroutine alertCoroutine;

    [Header("Score Panel UI")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private Image[] starImages; // Masukkan 3 gambar bintang di Inspector
    [SerializeField] private Sprite fullStarSprite;
    [SerializeField] private Sprite emptyStarSprite;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartTimer();
        if (alertOverlay != null)
        {
            // Pastikan overlay merah transparan di awal game
            Color c = alertOverlay.color;
            c.a = 0f;
            alertOverlay.color = c;
        }
        if (scorePanel != null) scorePanel.SetActive(false);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();

            // Opsional: Jika waktu lewat dari 3 menit (180 detik), game over otomatis
            if (elapsedTime >= maxTimeLimit)
            {
                StopTimer();
                TriggerGameOver();
            }
        }
    }

    #region TIMER FUNCTIONS
    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    #endregion

    #region ALERT SCREEN EFFECT (VIGNETTE RED)
    // Panggil fungsi ini jika status pemain "Ketahuan" oleh Penjaga
    public void SetAlertState(bool active)
    {
        if (isAlertActive == active) return;
        isAlertActive = active;

        if (isAlertActive)
        {
            if (alertCoroutine != null) StopCoroutine(alertCoroutine);
            alertCoroutine = StartCoroutine(PulseAlertEffect());
        }
        else
        {
            if (alertCoroutine != null) StopCoroutine(alertCoroutine);
            // Kembalikan ke transparan
            Color c = alertOverlay.color;
            c.a = 0f;
            alertOverlay.color = c;
        }
    }

    private IEnumerator PulseAlertEffect()
    {
        while (isAlertActive)
        {
            // Efek berkedip (pulsing) memanfaatkan fungsi Sinus
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; 
            // Batasi alpha maksimal 0.4f agar tidak terlalu menutupi layar game
            alpha = Mathf.Clamp(alpha, 0f, 0.4f); 

            Color c = alertOverlay.color;
            c.a = alpha;
            alertOverlay.color = c;
            yield return null;
        }
    }
    #endregion

    #region WIN & SCORING LOGIC
    // Panggil fungsi ini saat Maling menyentuh Pintu Exit setelah mengambil semua lukisan
    public void LevelCompleted()
    {
        StopTimer();
        if (scorePanel != null) scorePanel.SetActive(true);

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        finalTimeText.text = "Waktu: " + string.Format("{0:00}:{1:00}", minutes, seconds);

        int starsEarned = CalculateStars();
        DisplayStars(starsEarned);
    }

    private int CalculateStars()
    {
        if (elapsedTime <= threeStarsLimit) return 3;       // Di bawah 1 Menit
        else if (elapsedTime <= twoStarsLimit) return 2;     // 1 - 2 Menit
        else if (elapsedTime <= maxTimeLimit) return 1;      // 2 - 3 Menit
        else return 0;                                       // Lebih dari 3 Menit
    }

    private void DisplayStars(int starsCount)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < starsCount)
                starImages[i].sprite = fullStarSprite;
            else
                starImages[i].sprite = emptyStarSprite;
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Waktu habis! Misi Gagal.");
        // Hubungkan ke sistem Game Over timmu di sini
    }
    #endregion
}