using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour
{
    public float moveSpeed = 0.5f;                 // 기본 이동 속도
    float detectionRadius = 2f;             // 감지 반경
    public float fleeDistance = 3f;                 // 도망치는 거리
    public float checkInterval = 0.5f;              // 감지 간격

    [Header("배회 영역 설정 (월드 좌표 기준)")]
    private Vector3 wanderAreaCenter = new Vector3(-15,0,-25); // 배회 영역 중심
    Vector2 wanderAreaSize = new Vector2(50f, 30f); // 배회 영역 크기 (X, Z)

    private Vector3 targetPosition;
    private bool isFleeing = false;
    private bool isAttached = false;             // 플레이어에게 붙었는지 여부
    private Coroutine currentRoutine;

    private float myRadius;

    private void Awake()
    {
        // 자신의 크기를 PickupObject로부터 가져오기
        PickupObject pickup = GetComponent<PickupObject>();
        myRadius = pickup != null ? pickup.GetSizeValue() / 2f : 0.5f;

        // 부착 이벤트에 반응하도록 설정
        if (pickup != null)
        {
            pickup.onAttached += OnAttachedToPlayer;
        }
    }

    private void Start()
    {
        currentRoutine = StartCoroutine(WanderRoutine());
        InvokeRepeating(nameof(CheckForPlayer), 0f, checkInterval);
    }

    private void CheckForPlayer()
    {
        if (isAttached) return;  // 이미 붙었으면 무시

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        float distance = Vector3.Distance(transform.position, playerObj.transform.position);

        if (distance <= detectionRadius)
        {
            PlayerController player = playerObj.GetComponent<PlayerController>();
            if (player != null && player.GetRadius() > myRadius)
            {
                if (!isFleeing)
                {
                    if (currentRoutine != null) StopCoroutine(currentRoutine);
                    isFleeing = true;
                    currentRoutine = StartCoroutine(FleeRoutine(playerObj.transform));
                }
            }
        }
        else
        {
            if (isFleeing && distance > fleeDistance)
            {
                isFleeing = false;
                if (currentRoutine != null) StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(WanderRoutine());
            }
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (!isFleeing && !isAttached)
        {
            // 사각형 영역 내 랜덤 좌표 생성 (XZ 평면)
            float randomX = Random.Range(-wanderAreaSize.x * 0.5f, wanderAreaSize.x * 0.5f);
            float randomZ = Random.Range(-wanderAreaSize.y * 0.5f, wanderAreaSize.y * 0.5f);

            targetPosition = new Vector3(wanderAreaCenter.x + randomX, transform.position.y, wanderAreaCenter.z + randomZ);

            while (!isFleeing && !isAttached && Vector3.Distance(transform.position, targetPosition) > 0.2f)
            {
                Vector3 dir = (targetPosition - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private IEnumerator FleeRoutine(Transform threat)
    {
        while (isFleeing && !isAttached)
        {
            Debug.Log("플레이어 감지해서 도망 중");
            Vector3 awayDir = (transform.position - threat.position).normalized;
            transform.position += awayDir * moveSpeed * 2f * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 플레이어에게 붙었을 때 호출됨
    /// </summary>
    private void OnAttachedToPlayer()
    {
        isAttached = true;
        isFleeing = false;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        CancelInvoke(nameof(CheckForPlayer)); // 감지 중지
    }

    private void OnDrawGizmosSelected()
    {
        // 감지 반경 원형 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 배회 영역 사각형 (청록색)
        Gizmos.color = Color.cyan;

        Vector3 halfSize = new Vector3(wanderAreaSize.x * 0.5f, 0, wanderAreaSize.y * 0.5f);

        Vector3 topLeft = wanderAreaCenter + new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 topRight = wanderAreaCenter + new Vector3(halfSize.x, 0, halfSize.z);
        Vector3 bottomLeft = wanderAreaCenter + new Vector3(-halfSize.x, 0, -halfSize.z);
        Vector3 bottomRight = wanderAreaCenter + new Vector3(halfSize.x, 0, -halfSize.z);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
