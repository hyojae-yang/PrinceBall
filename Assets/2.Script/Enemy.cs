using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 플레이어보다 일정 크기 이상일 경우 플레이어의 오브젝트를 분리시키는 에너미
/// + 자동 왕복 이동
/// + PickupObject 분리 기능 포함
/// + 플레이어 충돌 시 튕겨져 나가는 힘 적용 (리지드바디 유무 고려)
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("플레이어보다 얼마나 커야 반응하는가")]
    public float sizeThresholdMultiplier = 1.2f;

    [Header("자동 이동 설정")]
    public Vector3 moveDirection = Vector3.forward;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    [Header("분리 쿨다운 설정")]
    public float dropCooldown = 1f;

    [Header("충돌 튕겨내기 힘 세기")]
    public float bounceForce;

    private Vector3 startPos;
    private int moveDirSign = 1;

    private PickupObject pickup;
    private bool hasRecentlyTriggered = false;

    // 플레이어에 붙었는지 여부
    private bool isAttached = false;

    private void Awake()
    {
        pickup = GetComponent<PickupObject>();
        startPos = transform.position;
    }

    private void Update()
    {
        if (isAttached)
            return;

        MoveEnemy();
    }

    /// <summary>
    /// 일직선 왕복 이동
    /// </summary>
    private void MoveEnemy()
    {
        Vector3 dirNormalized = moveDirection.normalized;
        Vector3 offset = dirNormalized * moveSpeed * moveDirSign * Time.deltaTime;
        transform.position += offset;

        Vector3 toCurrent = transform.position - startPos;
        float projectedDistance = Vector3.Dot(toCurrent, dirNormalized);

        if (projectedDistance > moveDistance)
        {
            moveDirSign = -1;
            transform.position = startPos + dirNormalized * moveDistance;
            transform.rotation = Quaternion.Euler(0f, 0f, 270f);
        }
        else if (projectedDistance < 0f)
        {
            moveDirSign = 1;
            transform.position = startPos;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasRecentlyTriggered || isAttached)
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        float playerRadius = player.GetRadius();
        float myRadius = pickup.GetSizeValue() / 2f;

        if (myRadius > playerRadius * sizeThresholdMultiplier)
        {
            Debug.Log("플레이어보다 크므로 분리 조건 충족!");

            // DropFromPlayer가 List<Transform> 반환하도록 수정 후,
            List<Transform> droppedObjects = DropFromPlayer(player);

            // 분리된 오브젝트만 튕겨내기
            BounceObjects(player, droppedObjects);

            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
        else
        {
            Debug.Log("플레이어보다 작아서 무시됨");
        }
    }


    /// <summary>
    /// 플레이어에 붙은 오브젝트들 중 일부를 랜덤하게 떼어냄
    /// </summary>
    private List<Transform> DropFromPlayer(PlayerController player)
    {
        GameObject playerObj = player.gameObject;
        List<PickupObject> pickups = new List<PickupObject>();

        // 플레이어 자식 중 PickupObject 컴포넌트가 있는 것들을 수집
        foreach (Transform child in playerObj.transform)
        {
            var pickup = child.GetComponent<PickupObject>();
            if (pickup != null)
                pickups.Add(pickup);
        }

        if (pickups.Count == 0)
        {
            return new List<Transform>(); // 분리할 게 없으면 빈 리스트 반환
        }

        int dropCount = Mathf.Min(5, pickups.Count);
        var randomPickups = pickups.OrderBy(x => Random.value).Take(dropCount).ToList();

        float totalShrinkAmount = 0f;
        List<Transform> droppedObjects = new List<Transform>(); // 분리된 오브젝트 리스트

        foreach (var pickup in randomPickups)
        {
            Transform obj = pickup.transform;
            obj.parent = null;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                                            // 튕기는 힘은 여기서 제거합니다
            }

            totalShrinkAmount += pickup.GetGrowthAmount();
            droppedObjects.Add(obj);

            Debug.Log($"랜덤 분리됨: {obj.name}");
        }

        if (totalShrinkAmount > 0f)
        {
            player.ShrinkBy(totalShrinkAmount);
            Debug.Log($"플레이어 크기 감소: {totalShrinkAmount}");
        }

        return droppedObjects; // 분리된 오브젝트 리스트 반환
    }


    /// <summary>
    /// 플레이어와 부딪친 후 튕겨져 나가게 하는 함수
    /// 리지드바디가 없으면 위치만 살짝 밀어내는 방식 적용
    /// </summary>
    private void BounceObjects(PlayerController player, List<Transform> droppedObjects)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        float scaledBounceForce = bounceForce * player.GetRadius();
        // 플레이볼 튕겨질 방향에 Y축 위쪽 성분 추가 (0.5f 정도 올려주기)
        Vector3 bounceDir = (player.transform.position - transform.position).normalized + Vector3.up * 0.5f;
        bounceDir.Normalize();

        // 플레이어 볼 튕겨내기
        if (playerRb != null)
        {
            playerRb.AddForce(bounceDir * scaledBounceForce, ForceMode.Impulse);
        }
        else
        {
            player.transform.position += bounceDir * 0.5f;
        }

        foreach (var objTransform in droppedObjects)
        {
            Rigidbody rb = objTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 플레이어 중심을 기준으로 방향 계산
                Vector3 baseDir = (objTransform.position - player.transform.position).normalized;

                // 랜덤 확산 추가 (더 자연스럽게)
                Vector3 randomSpread = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(0.6f, 1.0f),  // 위로 튕기는 성분 강조
                    Random.Range(-0.3f, 0.3f)
                );

                Vector3 forceDir = (baseDir + randomSpread).normalized;

                rb.isKinematic = false;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                // 살짝 띄워서 박힘 방지
                objTransform.position += Vector3.up * 0.2f;

                // 스케일링된 힘 적용
                rb.AddForce(forceDir * scaledBounceForce, ForceMode.Impulse);
            }
            else
            {
                // 리지드바디 없는 경우도 비슷한 방향으로 약하게 밀기
                Vector3 fallbackDir = (objTransform.position - player.transform.position).normalized + Vector3.up * 0.5f;
                objTransform.position += fallbackDir.normalized * 0.3f;
            }
        }

    }



    private void ResetTrigger()
    {
        hasRecentlyTriggered = false;
    }

    /// <summary>
    /// 플레이어에 붙을 때 호출되는 메서드 (기능 정지용)
    /// </summary>
    public void OnAttachedToPlayer()
    {
        isAttached = true;
        StopAllCoroutines();
    }
}
