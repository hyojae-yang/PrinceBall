using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 플레이어와 충돌 시, 플레이어보다 일정 크기 이상이면 반응하는 '분리자' 역할의 오브젝트
/// + 일직선 왕복 자동 이동 기능 포함
/// + PickupObject 분리 기능 포함
/// </summary>
[RequireComponent(typeof(PickupObject))]
public class Enemy : MonoBehaviour
{
    [Header("플레이어보다 얼마나 커야 반응하는가")]
    public float sizeThresholdMultiplier = 1.2f; // 플레이어보다 몇 배 이상 크면 반응할지

    [Header("자동 이동 설정")]
    public Vector3 moveDirection = Vector3.forward; // 로컬 기준 이동 방향
    public float moveDistance = 3f;                 // 이동 거리 (한 방향 최대 거리)
    public float moveSpeed = 2f;                    // 이동 속도 (m/s)

    [Header("분리 쿨다운 설정")]
    public float dropCooldown = 1f;                 // 분리 후 몇 초간 다시 분리 못하게 막을지

    private Vector3 startPos;
    private int moveDirSign = 1;

    private PickupObject pickup;
    private bool hasRecentlyTriggered = false;

    private void Awake()
    {
        pickup = GetComponent<PickupObject>();
        startPos = transform.position;
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// 일직선 왕복 이동
    /// </summary>
    private void MoveEnemy()
    {
        Vector3 dirNormalized = moveDirection.normalized;

        // 이동 방향 * 속도 * 시간 * 부호
        Vector3 offset = dirNormalized * moveSpeed * moveDirSign * Time.deltaTime;
        transform.position += offset;

        Vector3 toCurrent = transform.position - startPos;
        float projectedDistance = Vector3.Dot(toCurrent, dirNormalized);

        if (projectedDistance > moveDistance)
        {
            moveDirSign = -1;
            transform.position = startPos + dirNormalized * moveDistance;
            transform.rotation = Quaternion.Euler(0f, 0f, 270f); // 반대방향
        }
        else if (projectedDistance < 0f)
        {
            moveDirSign = 1;
            transform.position = startPos;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);  // 원래 방향
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasRecentlyTriggered)
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        float playerRadius = player.GetRadius();
        float myRadius = pickup.GetSizeValue() / 2f;

        if (myRadius > playerRadius * sizeThresholdMultiplier)
        {
            Debug.Log("플레이어보다 크므로 분리 조건 충족!");

            // 분리 기능 실행
            DropFromPlayer(player.gameObject);

            // 중복 방지 쿨다운
            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
        else
        {
            Debug.Log("플레이어보다 작아서 무시됨");
        }
    }

/// <summary>
/// 플레이어에 붙은 자식 오브젝트 중 일부를 랜덤하게 떼어냅니다.
/// </summary>
private void DropFromPlayer(GameObject playerObj)
{
    // 플레이어의 자식 중 PickupObject 컴포넌트를 가진 오브젝트만 수집
    List<PickupObject> pickups = new List<PickupObject>();

    foreach (Transform child in playerObj.transform)
    {
        var pickup = child.GetComponent<PickupObject>();
        if (pickup != null)
            pickups.Add(pickup);
    }

    if (pickups.Count == 0)
    {
        Debug.Log("분리할 오브젝트가 없음");
        return;
    }

    // 분리할 개수 (예: 3개까지)
    int dropCount = Mathf.Min(5, pickups.Count);

    // 리스트를 랜덤하게 섞고, 일부만 선택
    var randomPickups = pickups.OrderBy(x => Random.value).Take(dropCount).ToList();

    foreach (var pickup in randomPickups)
    {
        Transform obj = pickup.transform;

        obj.parent = null; // 부모에서 분리

        // 리지드바디 물리 활성화
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        Debug.Log($"랜덤 분리됨: {obj.name}");
    }
}


/// <summary>
/// 일정 시간 후 다시 반응 가능하게 초기화
/// </summary>
private void ResetTrigger()
    {
        hasRecentlyTriggered = false;
    }
}
