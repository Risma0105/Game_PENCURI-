using UnityEngine;

public class GuardAI : MonoBehaviour
{
    public enum GuardState { Patroli, Mengejar }

    [Header("AI State")]
    public GuardState currentGuardState = GuardState.Patroli;

    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public Transform[] waypoints;

    private int currentWaypointIndex = 0;

    [Header("Vision Settings")]
    public float viewDistance = 3.5f;
    public LayerMask playerLayer;
    public PlayerStateController playerScript;

    private SpriteRenderer spriteRenderer;
    private Vector2 arahHadapSenter = Vector2.right;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerScript == null)
        {
            playerScript = FindFirstObjectByType<PlayerStateController>();
        }
    }

    void Update()
    {
        if (playerScript == null) return;

        // =========================
        // DETEKSI PLAYER
        // =========================

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            playerScript.transform.position);

        // Hanya mendeteksi jika player berada dalam jarak pandang
        if (distanceToPlayer <= viewDistance)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                arahHadapSenter,
                viewDistance,
                playerLayer);

            if (hit.collider != null &&
                hit.collider.CompareTag("Player") &&
                playerScript.currentStatus ==
                PlayerStateController.State.Maling)
            {
                currentGuardState = GuardState.Mengejar;

                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.SetAlertState(true);
                }
            }
        }

        // =========================
        // PLAYER BERUBAH JADI PATUNG
        // =========================

        if (currentGuardState == GuardState.Mengejar &&
            playerScript.currentStatus ==
            PlayerStateController.State.Patung)
        {
            currentGuardState = GuardState.Patroli;

            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.SetAlertState(false);
            }
        }

        // =========================
        // PERGERAKAN HANSIP
        // =========================

        if (currentGuardState == GuardState.Mengejar)
        {
            KejarMaling();
        }
        else
        {
            if (waypoints.Length > 0)
            {
                Patroli();
            }
        }
    }

    void Patroli()
    {
        Transform targetWaypoint =
            waypoints[currentWaypointIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime);

        Vector2 arahJalan =
            (targetWaypoint.position -
             transform.position).normalized;

        AturArahHadap(arahJalan);

        if (Vector2.Distance(
            transform.position,
            targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex =
                (currentWaypointIndex + 1) %
                waypoints.Length;
        }
    }

    void KejarMaling()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerScript.transform.position,
            chaseSpeed * Time.deltaTime);

        Vector2 arahKePlayer =
            (playerScript.transform.position -
             transform.position).normalized;

        AturArahHadap(arahKePlayer);
    }

    void AturArahHadap(Vector2 arah)
    {
        if (arah.x > 0.05f)
        {
            spriteRenderer.flipX = false;
            arahHadapSenter = Vector2.right;
        }
        else if (arah.x < -0.05f)
        {
            spriteRenderer.flipX = true;
            arahHadapSenter = Vector2.left;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color =
            (currentGuardState == GuardState.Mengejar)
            ? Color.red
            : Color.yellow;

        Vector3 arahGaris =
            Application.isPlaying
            ? (Vector3)arahHadapSenter
            : Vector3.right;

        Gizmos.DrawRay(
            transform.position,
            arahGaris * viewDistance);
    }
}