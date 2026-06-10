using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public enum State { Maling, Patung }
    [Header("AI State")]
    public State currentStatus = State.Maling;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f; 

    [Header("Sprint & Stamina")]
    public float stamina = 10f; 
    public float maxStamina = 10f;
    private bool isSprinting = false; 

    [Header("Visual & Material Settings")]
    public Material defaultMaterial;   
    public Material crystalMaterial;   
    
    private SpriteRenderer sr;
    private Animator anim; 
    private Rigidbody2D rb; // <--- TAMBAHAN: Untuk mengunci fisik maling
    private Vector2 pergerakanInput; // Menyimpan data input WASD

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody2D>(); // <--- TAMBAHAN: Ambil komponen otomatis

        // Setting awal Rigidbody2D biar pas jalannya enak dan gak melorot
        if (rb != null) {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f; // Biar maling gak jatuh ke bawah layar karena gravitasi
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Biar maling gak guling-guling pas nabrak tembok
        }

        if (defaultMaterial == null && sr != null) {
            defaultMaterial = sr.material;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleState();
        }

        if (currentStatus == State.Maling && Input.GetKeyDown(KeyCode.R)) {
            if (stamina > 0) {
                SetSprintState(!isSprinting); 
            }
        }

        // Ambil input di Update agar responsif
        if (currentStatus == State.Maling) {
            float h = Input.GetAxisRaw("Horizontal"); 
            float v = Input.GetAxisRaw("Vertical");
            pergerakanInput = new Vector2(h, v).normalized;

            // Logika Animasi Jalan
            if (anim != null) {
                if (pergerakanInput.magnitude > 0 && !isSprinting) {
                    anim.SetBool("isWalking", true);
                } else {
                    anim.SetBool("isWalking", false);
                }
            }

            // Logika Flip Badan
            if (sr != null) {
                if (h > 0) sr.flipX = false;
                else if (h < 0) sr.flipX = true;
            }
        } else {
            pergerakanInput = Vector2.zero; // Jika jadi patung, kosongkan input jalannya
        }

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
    }

    // PERBAIKAN UTAMA: Pergerakan berbasis fisik ditaruh di FixedUpdate agar mentok dinding abu-abu
    void FixedUpdate() {
        if (currentStatus == State.Maling) {
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            // Jika tombol R lari aktif tapi player lepas keyboard, paksa gerak ke kanan (sesuai logic lamamu)
            if (isSprinting && pergerakanInput.magnitude == 0) {
                Vector2 posisiBaru = rb.position + Vector2.right * currentSpeed * Time.fixedDeltaTime;
                rb.MovePosition(posisiBaru);
            } else {
                Vector2 posisiBaru = rb.position + pergerakanInput * currentSpeed * Time.fixedDeltaTime;
                rb.MovePosition(posisiBaru);
            }
        } else {
            // Jika jadi patung, kunci posisinya biar gak bisa didorong hansip
            rb.linearVelocity = Vector2.zero;
        }
    }

    void SetSprintState(bool sprint) {
        isSprinting = sprint;
        if (anim != null) {
            anim.SetBool("isSprinting", isSprinting);
        }
        if (sr != null) {
            if (isSprinting) sr.material = crystalMaterial; 
            else sr.material = defaultMaterial; 
        }
    }

    void ToggleState() {
        if (currentStatus == State.Maling) {
            currentStatus = State.Patung;
            SetSprintState(false); 
            sr.color = Color.gray; 

            if (GameUIManager.Instance != null) {
                GameUIManager.Instance.SetAlertState(false);
            }
            Debug.Log("Status: Jadi Patung (Aman dari AI)");
        } else {
            currentStatus = State.Maling;
            sr.color = Color.white;
            Debug.Log("Status: Jadi Maling (Bisa Gerak)");
        }
    }
}