using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    // 1. Definisi status (Tugas FSM Risma - Minggu 2)
    public enum State { Maling, Patung }
    public State currentStatus = State.Maling;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f; 

    [Header("Sprint & Stamina (Rule 2.12)")]
    public float stamina = 10f; // Durasi lari maksimal 10 detik 
    public float maxStamina = 10f;
    private bool isSprinting = false;
    
    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        // Input ganti wujud menggunakan tombol E 
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleState();
        }

        // Hanya bisa gerak kalau statusnya Maling
        if (currentStatus == State.Maling) {
            HandleMovement();
        }
    }

    void ToggleState() {
        if (currentStatus == State.Maling) {
            currentStatus = State.Patung;
            sr.color = Color.gray; // Visual sementara saat jadi patung agar tidak terdeteksi
            Debug.Log("Status: Jadi Patung (Aman dari AI)");
        } else {
            currentStatus = State.Maling;
            sr.color = Color.white;
            Debug.Log("Status: Jadi Maling (Bisa Gerak)");
        }
    }

    void HandleMovement() {
        // Kontrol pergerakan menggunakan WASD atau Arrow Keys
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(h, v, 0).normalized;

        // Logika Sprint (Shift Kiri) 
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && move.magnitude > 0) {
            isSprinting = true;
            stamina -= Time.deltaTime; // Berkurang saat lari 
        } else {
            isSprinting = false;
            // Cooldown: Stamina terisi kembali saat tidak lari 
            if (stamina < maxStamina) {
                stamina += Time.deltaTime * 0.5f; 
            }
        }

        // Pastikan stamina tidak minus
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        transform.position += move * currentSpeed * Time.deltaTime;
    }
}