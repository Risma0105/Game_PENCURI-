using UnityEngine;
using UnityEngine.InputSystem; // WAJIB: Library untuk Input System baru

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
    private bool isSprinting = false; 

    [Header("Visual & Material Settings")]
    public Material defaultMaterial;   
    public Material crystalMaterial;   
    
    private SpriteRenderer sr;
    private Animator anim; 

    // Variabel baru untuk menyimpan arah pergerakan dari Input System
    private Vector2 moveInput;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 

        if (defaultMaterial == null && sr != null) {
            defaultMaterial = sr.material;
        }
    }

    void Update() {
        // Di Update sudah TIDAK ADA lagi Input.GetKeyDown atau Input.GetAxisRaw!
        if (currentStatus == State.Maling) {
            HandleMovement();
        }
    }

    // ====================================================================
    // INPUT SYSTEM NEW METHODS (Dipanggil otomatis oleh komponen Player Input)
    // ====================================================================

    // Menggantikan Input.GetAxisRaw("Horizontal" / "Vertical")
    public void OnMove(InputValue value) {
        if (currentStatus == State.Maling) {
            moveInput = value.Get<Vector2>();
        } else {
            moveInput = Vector2.zero;
        }
    }

    // Menggantikan Input.GetKeyDown(KeyCode.E)
    public void OnToggleState(InputValue value) {
        if (value.isPressed) {
            ToggleState();
        }
    }

    // Menggantikan Input.GetKeyDown(KeyCode.R)
    public void OnSprint(InputValue value) {
        if (value.isPressed && currentStatus == State.Maling) {
            if (stamina > 0) {
                SetSprintState(!isSprinting); 
            }
        }
    }

    // ====================================================================
    // CORE LOGIC (Logika game kamu tetap dipertahankan)
    // ====================================================================

    void HandleMovement() {
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0).normalized;

        // Animasi jalan
        if (anim != null) {
            if (move.magnitude > 0 && !isSprinting) {
                anim.SetBool("isWalking", true);
            } else {
                anim.SetBool("isWalking", false);
            }
        }

        // Logika Flip Badan berdasarkan moveInput.x (Horizontal)
        if (sr != null) {
            if (moveInput.x > 0) {
                sr.flipX = false;
            } else if (moveInput.x < 0) {
                sr.flipX = true;
            }
        }

        // Logika Stamina
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

    void SetSprintState(bool sprint) {
        isSprinting = sprint;

        if (anim != null) {
            anim.SetBool("isSprinting", isSprinting);
        }

        if (sr != null) {
            if (isSprinting) {
                sr.material = crystalMaterial; 
            } else {
                sr.material = defaultMaterial; 
            }
        }
    }

    void ToggleState() {
        if (currentStatus == State.Maling) {
            currentStatus = State.Patung;
            moveInput = Vector2.zero; // Stop pergerakan seketika saat jadi patung
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