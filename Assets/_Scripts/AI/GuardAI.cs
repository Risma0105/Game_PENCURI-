using UnityEngine;

public class GuardAI : MonoBehaviour {
    public enum GuardState { Patroli, Mengejar }
    [Header("AI State")]
    public GuardState currentGuardState = GuardState.Patroli;

    [Header("Patrol Settings")]
    public float moveSpeed = 2f;          // Kecepatan jalan biasa saat patroli
    public float chaseSpeed = 4.5f;       // Kecepatan lari saat ngejar maling
    public Transform[] waypoints;         // Titik-titik tujuan keliling
    private int currentWaypointIndex = 0; // Target titik saat ini

    [Header("Vision Settings")]
    public float viewDistance = 5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript;

    private SpriteRenderer spriteRenderer; 
    private Vector2 arahHadapSenter = Vector2.right; 
    private bool isPlayerSpotted = false;  // Status apakah player masuk raycast saat itu

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (playerScript == null) {
            playerScript = FindFirstObjectByType<PlayerStateController>();
        }
    }

    void Update() {
        // 1. LOGIKA SENTER (RAYCAST DETEKSI)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, arahHadapSenter, viewDistance, playerLayer);
        
        if (hit.collider != null) {
            // Cek status player dari script-mu
            if (playerScript.currentStatus == PlayerStateController.State.Maling) {
                isPlayerSpotted = true;
                currentGuardState = GuardState.Mengejar; // Otomatis berubah jadi ngejar!

                if (GameUIManager.Instance != null) {
                    GameUIManager.Instance.SetAlertState(true);
                }
            } else {
                // Kalau nemu player tapi statusnya Patung, Hansip anggap itu benda mati
                isPlayerSpotted = false; 
            }
        }
        else {
            isPlayerSpotted = false;
        }

        // Jika player merubah wujudnya jadi patung SAAT dikejar, Hansip kehilangan jejak
        if (currentGuardState == GuardState.Mengejar && playerScript.currentStatus == PlayerStateController.State.Patung) {
            currentGuardState = GuardState.Patroli; // Balik patroli lagi

            if (GameUIManager.Instance != null) {
                GameUIManager.Instance.SetAlertState(false);
            }
        }

        // 2. LOGIKA PERGERAKAN BERDASARKAN STATE HANSIP
        if (currentGuardState == GuardState.Mengejar) {
            KejarMaling();
        } else {
            if (waypoints.Length > 0) {
                Patroli();
            }
        }
    }

    void Patroli() {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Gerakkan hansip menuju titik target (pakai moveSpeed biasa)
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Hitung arah jalan & balik sprite
        Vector2 arahJalan = (targetWaypoint.position - transform.position).normalized;
        AturArahHadap(arahJalan);

        // Jika sudah sampai di titik target, ganti ke titik berikutnya
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f) {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void KejarMaling() {
        if (playerScript == null) return;

        // Gerakkan hansip menuju posisi koordinat Player saat ini (pakai chaseSpeed)
        transform.position = Vector2.MoveTowards(transform.position, playerScript.transform.position, chaseSpeed * Time.deltaTime);

        // Hitung arah lari menuju player agar senter dan spritenya selalu menghadap player
        Vector2 arahKePlayer = (playerScript.transform.position - transform.position).normalized;
        AturArahHadap(arahKePlayer);
    }

    // Fungsi pembantu untuk membalik arah sprite dan arah raycast senter
    void AturArahHadap(Vector2 arah) {
        if (arah.x > 0.05f) {
            spriteRenderer.flipX = false;    // Hadap kanan
            arahHadapSenter = Vector2.right; 
        } 
        else if (arah.x < -0.05f) {
            spriteRenderer.flipX = true;     // Hadap kiri
            arahHadapSenter = Vector2.left;  
        }
    }

    void OnDrawGizmos() {
        // Mengubah warna garis editor: Merah kalau ngejar, Kuning kalau patroli biasa
        Gizmos.color = (currentGuardState == GuardState.Mengejar) ? Color.red : Color.yellow;
        Vector3 arahGaris = Application.isPlaying ? (Vector3)arahHadapSenter : transform.right;
        Gizmos.DrawRay(transform.position, arahGaris * viewDistance);
    }
}