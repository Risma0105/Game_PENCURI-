using UnityEngine;

public class GuardAI : MonoBehaviour {
    public float rotationSpeed = 50f;
    public float viewDistance = 5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript; // Referensi ke script Risma

    void Update() {
        // AI berputar (rotasi) untuk menengok 
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Raycast untuk simulasi senter (simulasi cahaya senter penjaga)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, viewDistance, playerLayer);
        
        if (hit.collider != null) {
            // CEK KONDISI: Jika kena Player DAN statusnya MALING 
            if (playerScript.currentStatus == PlayerStateController.State.Maling) {
                Debug.Log("KETAHUAN! Misi Gagal."); // Sesuai konsep: Alert merah dan misi gagal
            } else {
                // Sesuai mekanik: Jika jadi patung, penjaga akan mengabaikan pemain
                Debug.Log("Hanya patung biasa..."); 
            }
        }
    }

    // Visualisasi jangkauan senter di editor agar Zahra mudah mengatur level
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * viewDistance);
    }
}