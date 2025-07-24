using UnityEngine;

/// <summary>
/// Dog 에너미 AI: 플레이어를 일정 시야각과 거리 안에서 감지하면 추적하고, 놓치면 원래 자리로 돌아갑니다.
/// </summary>
public class Dog : Enemy
{
    [Header("플레이어 추적 설정")]
    public float viewAngle = 90f;             // 시야각 (도)
    public float viewDistance = 5f;           // 시야 거리
    public float moveSpeed = 3f;              // 도그 이동 속도
    private float stopDistance = 0.1f;        // 복귀 완료 판정용 거리

    private Transform player;                 // 플레이어 트랜스폼
    private Vector3 startPos;                 // 도그가 태어난 위치
    private Quaternion startRotation; // 도그가 태어난 방향
    private bool isReturning = false;         // 복귀 중인지 여부

    /// <summary>
    /// 초기화
    /// </summary>
    protected new void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    /// <summary>
    /// 매 프레임마다 호출
    /// </summary>
    void Update()
    {
        if (isAttached || player == null)
            return;

        if (IsPlayerInSight())
        {
            isReturning = false;
            MoveToPlayer();
        }
        else
        {
            ReturnToStart();
        }
    }

    /// <summary>
    /// 플레이어가 시야각과 거리 내에 있는지만 판단
    /// </summary>
    bool IsPlayerInSight()
    {
        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // 1. 거리 체크
        if (distanceToPlayer > viewDistance)
            return false;

        // 2. 시야각 체크
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > viewAngle * 0.5f)
            return false;
        

        // 3. 장애물 체크 (레이캐스트)
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, toPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, viewDistance))
        {
            if (hit.transform != player)
            {
                // 레이캐스트가 플레이어보다 먼저 다른 오브젝트에 맞았음 → 가려졌음
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 플레이어 쪽으로 이동
    /// </summary>
    void MoveToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 자연스럽게 회전
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }

    /// <summary>
    /// 원래 위치로 복귀
    /// </summary>
    void ReturnToStart()
    {
        if (!isReturning)
            isReturning = true;

        Vector3 toStart = startPos - transform.position;
        float distance = toStart.magnitude;

        if (distance < stopDistance)
        {
            isReturning = false;
            transform.rotation = startRotation; // 방향 복원
            return;
        }

        Vector3 dir = toStart.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // 자연스럽게 회전
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Scene 뷰에서 시야 범위 기즈모 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance); // 전체 탐지 거리

        // 부채꼴 시야 시각화
        Vector3 forward = transform.forward;
        Quaternion leftRot = Quaternion.Euler(0, -viewAngle / 2f, 0);
        Quaternion rightRot = Quaternion.Euler(0, viewAngle / 2f, 0);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * viewDistance);
    }
#endif
}
