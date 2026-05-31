using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public enum State { Maling, Patung }
    public State currentStatus = State.Maling;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f; 

    [Header("Sprint & Stamina")]
    public float stamina = 10f; // Durasi lari maksimal 10 detik
    public float maxStamina = 10f;
    private bool isSprinting = false; // Sekarang dikontrol oleh tombol R

    [Header("Visual & Material Settings")]
    public Material defaultMaterial;   // Seret material standar (Sprite-Lit-Default) ke sini
    public Material crystalMaterial;   // Seret material kristal/es kamu ke sini[cite: 1]
    
    private SpriteRenderer sr;
    private Animator anim; // Komponen untuk memicu animasi lari di Unity[cite: 1]

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // Mengambil komponen Animator otomatis saat game mulai[cite: 1]

        // Menangkap material default otomatis kalau slotnya kosong[cite: 1]
        if (defaultMaterial == null && sr != null) {
            defaultMaterial = sr.material;
        }
    }

    void Update() {
        // Input ganti wujud menggunakan tombol E[cite: 1]
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleState();
        }

        // Aktifkan/Matikan lari otomatis menggunakan tombol R (Hanya saat jadi Maling)[cite: 1]
        if (currentStatus == State.Maling && Input.GetKeyDown(KeyCode.R)) {
            if (stamina > 0) {
                SetSprintState(!isSprinting); // Panggil fungsi ganti status lari & animasi[cite: 1]
            }
        }

        // Hanya bisa gerak kalau statusnya Maling[cite: 1]
        if (currentStatus == State.Maling) {
            HandleMovement();
        }
    }

    // FUNGSI BARU: Untuk mengatur animasi dan material saat tombol R ditekan[cite: 1]
    void SetSprintState(bool sprint) {
        isSprinting = sprint;

        // Picu parameter isSprinting di Animator Unity biar animasinya berubah lari/idle[cite: 1]
        if (anim != null) {
            anim.SetBool("isSprinting", isSprinting);
        }

        // Logika ganti material kristal secara real-time[cite: 1]
        if (sr != null) {
            if (isSprinting) {
                sr.material = crystalMaterial; // Berubah jadi material kristal/es saat lari[cite: 1]
            } else {
                sr.material = defaultMaterial; // Kembali ke material normal saat diam[cite: 1]
            }
        }
    }

    void ToggleState() {
        if (currentStatus == State.Maling) {
            currentStatus = State.Patung;
            SetSprintState(false); // Otomatis batal lari & balikin material ke normal kalau jadi patung[cite: 1]
            sr.color = Color.gray; 

            // ====================================================================
            // INTEGRASI UI: Efek layar merah langsung mati otomatis saat jadi patung[cite: 1]
            // ====================================================================
            if (GameUIManager.Instance != null) {
                GameUIManager.Instance.SetAlertState(false);
            }
            // ====================================================================

            Debug.Log("Status: Jadi Patung (Aman dari AI)[cite: 1]");
        } else {
            currentStatus = State.Maling;
            sr.color = Color.white;
            Debug.Log("Status: Jadi Maling (Bisa Gerak)[cite: 1]");
        }
    }

    void HandleMovement() {
        // Mengambil input WASD / Arrow Keys[cite: 1]
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 move = new Vector3(h, v, 0).normalized;

        // Logika Pengurangan & Pemulihan Stamina[cite: 1]
        if (isSprinting) {
            // Jika stamina habis, otomatis berhenti sprint[cite: 1]
            if (stamina > 0) {
                stamina -= Time.deltaTime; 
            } else {
                SetSprintState(false); // Stamina habis, panggil fungsi untuk reset animasi & material[cite: 1]
            }
        } else {
            // Pemulihan stamina saat tidak lari[cite: 1]
            if (stamina < maxStamina) {
                stamina += Time.deltaTime * 0.5f; 
            }
        }

        // Pastikan stamina tidak minus atau melebihi batas[cite: 1]
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        // Tentukan kecepatan berdasarkan status sprint[cite: 1]
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // JIKA sedang sprint, karakter akan otomatis lari ke arah depan[cite: 1]
        if (isSprinting && move.magnitude == 0) {
            transform.position += Vector3.right * currentSpeed * Time.deltaTime;
        } else {
            // Pergerakan manual biasa menggunakan WASD[cite: 1]
            transform.position += move * currentSpeed * Time.deltaTime;
        }
    }
}