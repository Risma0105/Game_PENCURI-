using UnityEngine;

public class GuardAI : MonoBehaviour {
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;          // Kecepatan jalan hansip
    public Transform[] waypoints;         // Titik-titik tujuan keliling
    private int currentWaypointIndex = 0; // Target titik saat ini

    [Header("Vision Settings")]
    public float viewDistance = 5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript;

    // Tambahkan ini untuk mengatur arah hadap gambar Hansip
    private SpriteRenderer spriteRenderer; 
    private Vector2 arahHadapSenter = Vector2.right; // Menampung arah senter (kanan/kiri)

    void Start() {
        // Mengambil komponen SpriteRenderer yang ada di objek Hansip
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        // 1. LOGIKA KELILING (PATROLI)
        if (waypoints.Length > 0) {
            Patroli();
        }

        // 2. LOGIKA SENTER (RAYCAST)
        // Sekarang menembak sesuai dengan arahHadapSenter (bukan transform.right lagi)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, arahHadapSenter, viewDistance, playerLayer);
        
        if (hit.collider != null) {
            if (playerScript.currentStatus == PlayerStateController.State.Maling) {
                // ====================================================================
                // INTEGRASI UI: Menyalakan efek layar merah berkedip (Alert)
                // ====================================================================
                if (GameUIManager.Instance != null) {
                    GameUIManager.Instance.SetAlertState(true);
                }
                // ====================================================================

                Debug.Log("KETAHUAN! Misi Gagal.");
            } else {
                Debug.Log("Hanya patung biasa..."); 
            }
        }
        else {
            // ====================================================================
            // INTEGRASI UI: Mematikan efek layar merah jika tidak ada objek terdeteksi
            // ====================================================================
            if (GameUIManager.Instance != null) {
                GameUIManager.Instance.SetAlertState(false);
            }
            // ====================================================================
        }
    }

    void Patroli() {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Gerakkan hansip menuju titik target
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Hitung arah jalan
        Vector2 arahJalan = (targetWaypoint.position - transform.position).normalized;

        // Cek arah jalan untuk membalik sprite (Hanya merubah arah X)
        if (arahJalan.x > 0.05f) {
            spriteRenderer.flipX = false;    // Hadap kanan (sesuai gambar asli)
            arahHadapSenter = Vector2.right; // Senter nembak ke kanan
        } 
        else if (arahJalan.x < -0.05f) {
            spriteRenderer.flipX = true;     // Hadap kiri (gambar dibalik otomatis oleh Unity)
            arahHadapSenter = Vector2.left;  // Senter nembak ke kiri
        }

        // Jika sudah sampai di titik target, ganti ke titik berikutnya
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f) {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Visualisasi jangkauan senter di Scene View Unity
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        // Menggunakan arahHadapSenter agar garis merah di Unity Editor juga ikut berbalik arah
        Vector3 arahGaris = Application.isPlaying ? (Vector3)arahHadapSenter : transform.right;
        Gizmos.DrawRay(transform.position, arahGaris * viewDistance);
    }
}