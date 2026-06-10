using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardAI : MonoBehaviour {
    public enum GuardState { Patroli, Idle, Mengejar }
    [Header("AI State")]
    public GuardState currentGuardState = GuardState.Patroli;

    [Header("Movement Settings (Follow Path)")]
    public float moveSpeed = 2f;       
    public float chaseSpeed = 4f;    
    
    [Header("Idle Settings")]
    public float idleDuration = 1.5f;       
    private bool isWaiting = false;       

    [Header("Vision Settings")]
    public float viewDistance = 5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript;

    [Header("Layer Path (Ubin Ungu)")]
    public LayerMask pathLayer; // <--- HANSIP HANYA BOLEH JALAN DI LAYER INI

    private SpriteRenderer spriteRenderer; 
    private Vector2 arahHadapSenter = Vector2.right; 
    private Vector2 arahJalanSekarang = Vector2.right;
    private Animator animator; 
    private Rigidbody2D rb;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        // Kunci fisik murni biar gak mental-mental
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (playerScript == null) {
            playerScript = FindFirstObjectByType<PlayerStateController>();
        }

        // Cari arah ubin ungu pertama untuk jalan
        CariArahUbinUngu();
    }

    void Update() {
        // DETEKSI SENTER KE MALING (PAPASAN = ALERT MERAH)
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, arahHadapSenter, viewDistance, playerLayer);
        
        if (hitPlayer.collider != null && hitPlayer.collider.gameObject.CompareTag("Player")) {
            if (playerScript != null && playerScript.currentStatus == PlayerStateController.State.Maling) {
                if (isWaiting) {
                    StopAllCoroutines();
                    isWaiting = false;
                }
                currentGuardState = GuardState.Mengejar; 
                if (GameUIManager.Instance != null) GameUIManager.Instance.SetAlertState(true);
            }
        }

        // JIKA MALING JADI PATUNG = ALERT MATI, KEMBALI PATROLI
        if (currentGuardState == GuardState.Mengejar && playerScript != null && playerScript.currentStatus == PlayerStateController.State.Patung) {
            currentGuardState = GuardState.Patroli; 
            if (GameUIManager.Instance != null) GameUIManager.Instance.SetAlertState(false);
            CariArahUbinUngu();
        }
    }

    void FixedUpdate() {
        if (currentGuardState == GuardState.Mengejar) {
            KejarMaling();
        } 
        else if (currentGuardState == GuardState.Patroli && !isWaiting) {
            PatroliIkutJalanUngu();
        }
    }

    void PatroliIkutJalanUngu() {
        if (animator != null) animator.SetBool("isWalking", true);

        // Cek ubin di depan posisi hansip (jarak 0.8f)
        Vector2 posisiCek = (Vector2)transform.position + arahJalanSekarang * 0.8f;
        Collider2D hitPath = Physics2D.OverlapPoint(posisiCek, pathLayer);

        // Jika di depan BUKAN ubin ungu (artinya ketemu dinding abu-abu atau ujung pink), hansip harus belok!
        if (hitPath == null) {
            StartCoroutine(TungguDanBelok());
            return;
        }

        // Jika jalan aman, maju terus mengikuti jalur ubin ungu
        Vector2 posisiBaru = rb.position + arahJalanSekarang * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(posisiBaru);
        AturArahHadap(arahJalanSekarang);
    }

    IEnumerator TungguDanBelok() {
        isWaiting = true;
        if (animator != null) animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(idleDuration);

        CariArahUbinUngu();
        isWaiting = false;
    }

    void CariArahUbinUngu() {
        Vector2[] empatArah = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
        List<Vector2> arahTersedia = new List<Vector2>();

        // Scan otomatis ke 4 arah mata angin, cari mana yang ada ubin ungunya
        foreach (Vector2 arah in empatArah) {
            Vector2 titikTarget = (Vector2)transform.position + arah * 1.0f;
            Collider2D hit = Physics2D.OverlapPoint(titikTarget, pathLayer);
            
            if (hit != null) {
                arahTersedia.Add(arah);
            }
        }

        if (arahTersedia.Count > 0) {
            // Biar gak bolak-balik pusing di tempat, prioritaskan jangan langsung putar balik ke belakang
            Vector2 arahMundur = -arahJalanSekarang;
            if (arahTersedia.Count > 1 && arahTersedia.Contains(arahMundur)) {
                arahTersedia.Remove(arahMundur);
            }
            // Pilih jalan ungu yang tersedia secara otomatis
            arahJalanSekarang = arahTersedia[Random.Range(0, arahTersedia.Count)];
        } else {
            // Kalau benar-benar buntu terpaksa balik kanan bubar jalan
            arahJalanSekarang = -arahJalanSekarang;
        }

        AturArahHadap(arahJalanSekarang);
    }

    void KejarMaling() {
        if (playerScript == null || rb == null) return;
        if (animator != null) animator.SetBool("isWalking", true);

        Vector2 arahKePlayer = (playerScript.transform.position - transform.position).normalized;
        Vector2 posisiMengejar = rb.position + arahKePlayer * chaseSpeed * Time.fixedDeltaTime;
        rb.MovePosition(posisiMengejar);
        AturArahHadap(arahKePlayer);
    }

    void AturArahHadap(Vector2 arah) {
        Vector3 skalaLokal = transform.localScale;
        if (arah.x > 0.05f) {
            skalaLokal.x = Mathf.Abs(skalaLokal.x); 
            arahHadapSenter = Vector2.right; 
        } 
        else if (arah.x < -0.05f) {
            skalaLokal.x = -Mathf.Abs(skalaLokal.x); 
            arahHadapSenter = Vector2.left;  
        }
        transform.localScale = skalaLokal;
    }

    void OnDrawGizmos() {
        Gizmos.color = (currentGuardState == GuardState.Mengejar) ? Color.red : Color.yellow;
        Vector3 arahGaris = Application.isPlaying ? (Vector3)arahHadapSenter : transform.right;
        Gizmos.DrawRay(transform.position, arahGaris * viewDistance);
    }
}