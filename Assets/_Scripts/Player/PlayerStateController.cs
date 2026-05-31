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
    public Material crystalMaterial;   // Seret material kristal/es kamu ke sini
    
    private SpriteRenderer sr;
    private Animator anim; // Komponen untuk memicu animasi di Unity

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // Mengambil komponen Animator otomatis saat game mulai

        // Menangkap material default otomatis kalau slotnya kosong
        if (defaultMaterial == null && sr != null) {
            defaultMaterial = sr.material;
        }
    }

    void Update() {
        // Input ganti wujud menggunakan tombol E
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleState();
        }

        // Aktifkan/Matikan lari otomatis menggunakan tombol R (Hanya saat jadi Maling)
        if (currentStatus == State.Maling && Input.GetKeyDown(KeyCode.R)) {
            if (stamina > 0) {
                SetSprintState(!isSprinting); // Panggil fungsi ganti status lari & animasi
            }
        }

        // Hanya bisa gerak kalau statusnya Maling
        if (currentStatus == State.Maling) {
            HandleMovement();
        }
    }

    // Mengatur animasi dan material saat tombol R ditekan atau saat stamina habis
    void SetSprintState(bool sprint) {
        isSprinting = sprint;

        // Picu parameter isSprinting di Animator Unity
        if (anim != null) {
            anim.SetBool("isSprinting", isSprinting);
        }

        // Logika ganti material kristal secara real-time
        if (sr != null) {
            if (isSprinting) {
                sr.material = crystalMaterial; // Berubah jadi material kristal/es saat lari
            } else {
                sr.material = defaultMaterial; // Kembali ke material normal saat diam
            }
        }
    }

    void ToggleState() {
        if (currentStatus == State.Maling) {
            currentStatus = State.Patung;
            SetSprintState(false); // Otomatis batal lari & balikin material ke normal kalau jadi patung
            sr.color = Color.gray; 

            // ====================================================================
            // INTEGRASI UI: Efek layar merah langsung mati otomatis saat jadi patung
            // ====================================================================
            if (GameUIManager.Instance != null) {
                GameUIManager.Instance.SetAlertState(false);
            }
            // ====================================================================

            Debug.Log("Status: Jadi Patung (Aman dari AI)");
        } else {
            currentStatus = State.Maling;
            sr.color = Color.white;
            Debug.Log("Status: Jadi Maling (Bisa Gerak)");
        }
    }

    void HandleMovement() {
        // Mengambil input WASD / Arrow Keys
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 move = new Vector3(h, v, 0).normalized;

        // ====================================================================
        // INTEGRASI ANIMASI JALAN & FLIP HADAP KANAN/KIRI AUTOMATIS
        // ====================================================================
        if (anim != null) {
            // Jika ada input gerakan, nyalakan animasi jalan biasa
            if (move.magnitude > 0 && !isSprinting) {
                anim.SetBool("isWalking", true);
            } else {
                anim.SetBool("isWalking", false);
            }
        }

        // LOGIKA FLIP BADAN:
        if (sr != null) {
            // Jika h > 0 (pencet D / gerak ke kanan), mukanya normal (hadap kanan)
            if (h > 0) {
                sr.flipX = false;
            }
            // Jika h < 0 (pencet W/A/gerak ke kiri), balik gambarnya biar hadap kiri
            else if (h < 0) {
                sr.flipX = true;
            }
        }
        // ====================================================================

        // Logika Pengurangan & Pemulihan Stamina
        if (isSprinting) {
            if (stamina > 0) {
                stamina -= Time.deltaTime; 
            } else {
                SetSprintState(false); 
            }
        } else {
            if (stamina < maxStamina) {
                stamina += Time.deltaTime * 0.5f; 
            }
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (isSprinting && move.magnitude == 0) {
            transform.position += Vector3.right * currentSpeed * Time.deltaTime;
        } else {
            transform.position += move * currentSpeed * Time.deltaTime;
        }
    }
}