using UnityEngine;

public class GuardAI : MonoBehaviour {
    public enum GuardState { Patroli, Mengejar }
    [Header("AI State")]
    public GuardState currentGuardState = GuardState.Patroli;

    [Header("Patrol Settings (Random)")]
    public float moveSpeed = 2f;          // Kecepatan jalan biasa saat patroli
    public float chaseSpeed = 4.5f;       // Kecepatan lari saat ngejar maling
    public float patrolRadius = 5f;       // Seberapa jauh jarak maksimal hansip boleh keluyuran dari posisi awalnya
    public float waktuTungguMin = 1f;     // Waktu diam minimum saat sampai di titik acak
    public float waktuTungguMax = 3f;     // Waktu diam maksimum sebelum jalan lagi

    [Header("Vision Settings")]
    public float viewDistance = 5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript;

    [Header("Proximity Alert Settings")]
    public float alertRadius = 3.5f;      // Jarak terdekat sebelum layar merah menyala

    private SpriteRenderer spriteRenderer; 
    private Vector2 arahHadapSenter = Vector2.right; 
    private bool isPlayerSpotted = false;  
    private float jedaMulaiDeteksi = 0.5f; 

    // Variabel internal untuk logika random
    private Vector2 posisiAwalHansip;
    private Vector2 targetPosisiAcak;
    private float timerTunggu = 0f;
    private bool sedangMenunggu = false;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentGuardState = GuardState.Patroli;

        // Catat posisi awal hansip ditaruh di map sebagai patokan pusat patroli acaknya
        posisiAwalHansip = transform.position;
        TentukanTargetAcakBaru();

        if (playerScript == null) {
            playerScript = FindFirstObjectByType<PlayerStateController>();
        }

        if (GameUIManager.Instance != null) {
            GameUIManager.Instance.SetAlertState(false);
        }
    }

    void Update() {
        // --- 1. LOGIKA SENTER (RAYCAST DETEKSI UNTUK MENGEJAR) ---
        if (Time.timeSinceLevelLoad > jedaMulaiDeteksi) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, arahHadapSenter, viewDistance, playerLayer);
            
            if (hit.collider != null) {
                if (playerScript.currentStatus == PlayerStateController.State.Maling) {
                    isPlayerSpotted = true;
                    currentGuardState = GuardState.Mengejar; 
                } else {
                    isPlayerSpotted = false; 
                }
            } else {
                isPlayerSpotted = false;
            }
        }

        if (currentGuardState == GuardState.Mengejar && playerScript.currentStatus == PlayerStateController.State.Patung) {
            currentGuardState = GuardState.Patroli; 
            TentukanTargetAcakBaru(); // Langsung cari target jalan baru pas kehilangan jejak
        }

        // --- 2. LOGIKA ALERT OVERLAY (BERDASARKAN JARAK RADIUS AMAN) ---
        if (playerScript != null) {
            float jarakKePlayer = Vector2.Distance(transform.position, playerScript.transform.position);

            if (playerScript.currentStatus == PlayerStateController.State.Maling && jarakKePlayer <= alertRadius) {
                if (GameUIManager.Instance != null) {
                    GameUIManager.Instance.SetAlertState(true); 
                }
            } 
            else if (currentGuardState != GuardState.Mengejar) {
                if (GameUIManager.Instance != null) {
                    GameUIManager.Instance.SetAlertState(false); 
                }
            }
        }

        // --- 3. LOGIKA PERGERAKAN ---
        if (currentGuardState == GuardState.Mengejar) {
            sedangMenunggu = false; // Batalkan tunggu jika mendadak harus ngejar
            KejarMaling();
        } else {
            PatroliAcak();
        }
    }

    void PatroliAcak() {
        if (sedangMenunggu) {
            timerTunggu -= Time.deltaTime;
            if (timerTunggu <= 0) {
                sedangMenunggu = false;
                TentukanTargetAcakBaru();
            }
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosisiAcak, moveSpeed * Time.deltaTime);

        // Hitung arah pandang wajah & senter
        Vector2 arahJalan = (targetPosisiAcak - (Vector2)transform.position).normalized;
        AturArahHadap(arahJalan);

        // Jika hansip sudah dekat banget dengan titik acak tersebut
        if (Vector2.Distance(transform.position, targetPosisiAcak) < 0.2f) {
            sedangMenunggu = true;
            // Hansip bakal diam selama beberapa detik (diacak antara min & max) sebelum nyari titik baru lagi
            timerTunggu = Random.Range(waktuTungguMin, waktuTungguMax);
        }
    }

    void TentukanTargetAcakBaru() {
        // Cari koordinat X dan Y acak di sekitar lingkaran area asalnya
        Vector2 arahAcak = Random.insideUnitCircle * patrolRadius;
        targetPosisiAcak = posisiAwalHansip + arahAcak;
    }

    void KejarMaling() {
        if (playerScript == null) return;
        transform.position = Vector2.MoveTowards(transform.position, playerScript.transform.position, chaseSpeed * Time.deltaTime);

        Vector2 arahKePlayer = (playerScript.transform.position - transform.position).normalized;
        AturArahHadap(arahKePlayer);
    }

    void AturArahHadap(Vector2 arah) {
        if (arah.x > 0.05f) {
            spriteRenderer.flipX = false;    
            arahHadapSenter = Vector2.right; 
        } 
        else if (arah.x < -0.05f) {
            spriteRenderer.flipX = true;     
            arahHadapSenter = Vector2.left;  
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = (currentGuardState == GuardState.Mengejar) ? Color.red : Color.yellow;
        Vector3 arahGaris = Application.isPlaying ? (Vector3)arahHadapSenter : transform.right;
        Gizmos.DrawRay(transform.position, arahGaris * viewDistance);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        // TAMBAHAN GIZMOS: Lingkaran Hijau menunjukkan batas maksimal area Hansip boleh keluyuran secara acak
        if (!Application.isPlaying) posisiAwalHansip = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(posisiAwalHansip, patrolRadius);
    }
}