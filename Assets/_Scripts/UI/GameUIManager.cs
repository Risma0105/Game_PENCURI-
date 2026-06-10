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
        GameState.keyCollected = false;
        GameState.objectiveCollected = false;

        StartTimer();
        if (alertOverlay != null)
        {
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
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    #endregion

    #region ALERT SCREEN EFFECT (VIGNETTE RED)
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
            if (alertOverlay != null)
            {
                Color c = alertOverlay.color;
                c.a = 0f;
                alertOverlay.color = c;
            }
        }
    }

    private IEnumerator PulseAlertEffect()
    {
        while (isAlertActive && alertOverlay != null)
        {
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; 
            alpha = Mathf.Clamp(alpha, 0f, 0.4f); 

            Color c = alertOverlay.color;
            c.a = alpha;
            alertOverlay.color = c;
            yield return null;
        }
    }
    #endregion

    #region WIN & SCORING LOGIC
    public void LevelCompleted()
    {
        Debug.Log("LEVEL COMPLETE DIPANGGIL");
        StopTimer();
        
        // PENGAMAN 1: Nyalakan panel skor jika tidak kosong
        if (scorePanel != null) 
        {
            scorePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Score Panel belum di-drag ke dalam _UIManager di Inspector!");
        }

        // PENGAMAN 2: Update teks waktu final
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            finalTimeText.text = "Waktu: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        int starsEarned = CalculateStars();
        DisplayStars(starsEarned);
    }

    private int CalculateStars()
    {
        if (elapsedTime <= threeStarsLimit) return 3;       
        else if (elapsedTime <= twoStarsLimit) return 2;     
        else if (elapsedTime <= maxTimeLimit) return 1;      
        else return 0;                                       
    }

    private void DisplayStars(int starsCount)
    {
        // PENGAMAN 3: Memastikan array bintang tidak kosong dan komponen gambarnya ada
        if (starImages == null || starImages.Length == 0)
        {
            Debug.LogWarning("Star Images array kosong! Harap masukkan object Star_1, Star_2, Star_3 ke Inspector.");
            return;
        }

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                if (i < starsCount)
                {
                    if (fullStarSprite != null) starImages[i].sprite = fullStarSprite;
                }
                else
                {
                    if (emptyStarSprite != null) starImages[i].sprite = emptyStarSprite;
                }
            }
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Waktu habis! Misi Gagal.");
    }
    #endregion
}