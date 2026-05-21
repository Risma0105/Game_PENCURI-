using UnityEngine;

public class GuardAI : MonoBehaviour {
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;          // Kecepatan jalan hansip
    public Transform[] waypoints;         // Titik-titik tujuan keliling (taruh objek kosong di Unity)
    private int currentWaypointIndex = 0; // Target titik saat ini

    [Header("Vision Settings")]
    public float viewDistance = 5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript;

    void Update() {
        // 1. LOGIKA KELILING (PATROLI)
        if (waypoints.Length > 0) {
            Patroli();
        }

        // 2. LOGIKA SENTER (RAYCAST)
        // Menembakkan senter ke arah hansip menghadap (transform.right)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, viewDistance, playerLayer);
        
        if (hit.collider != null) {
            if (playerScript.currentStatus == PlayerStateController.State.Maling) {
                Debug.Log("KETAHUAN! Misi Gagal.");
            } else {
                Debug.Log("Hanya patung biasa..."); 
            }
        }
    }

    void Patroli() {
        // Ambil posisi target titik saat ini
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Gerakkan hansip menuju titik target
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Hitung arah jalan (buat nentuin arah hadap sprite dan senter)
        Vector2 arahJalan = (targetWaypoint.position - transform.position).normalized;

        if (arahJalan.magnitude > 0) {
            // Mengubah arah transform.right ke arah tujuan jalan (Senter otomatis ikut)
            transform.right = arahJalan;
        }

        // Jika sudah sampai di titik target, ganti ke titik berikutnya
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f) {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Visualisasi jangkauan senter di Scene View Unity
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * viewDistance);
    }
}