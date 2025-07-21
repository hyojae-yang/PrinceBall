using UnityEngine;

/// <summary>
/// 플레이어 구체 이동 및 오브젝트 붙임 기능 담당
/// 실제 구체 크기는 유지하며, 콜라이더 반경만 증가
/// 붙은 오브젝트 크기에 따라 성장
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 5f;

    [Header("붙임 크기 비율 (작을수록 큰 오브젝트가 쉽게 붙음)")]
    [SerializeField] private float attachSizeRatio = 1.5f;

    [Header("오브젝트 크기당 반지름 성장 비율")]
    [SerializeField] private float growthRatio = 0.04f;

    private Rigidbody rb;
    private SphereCollider sphereCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();

        // 회전 물리 제한 (회전은 직접 transform으로 처리)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0, v).normalized;
        if (input.sqrMagnitude == 0f) return;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // y축 제거 후 정규화해서 방향 보정
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 카메라 기준 입력 방향 계산
        Vector3 moveDir = camForward * v + camRight * h;
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + moveOffset);

        // 공 회전 시뮬레이션 (접지 회전 축 기준)
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    /// <summary>
    /// 현재 플레이어의 월드 반지름 (스케일 반영)
    /// </summary>
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        float playerDiameter = GetRadius() * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            AttachObject(collision.gameObject, objectSize);
        }
    }

    /// <summary>
    /// 충돌한 오브젝트를 플레이어에 부착
    /// </summary>
    private void AttachObject(GameObject obj, float objectSize)
    {
        // Rigidbody 비활성화 (물리 영향 제거)
        if (obj.TryGetComponent<Rigidbody>(out var objRb))
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }

        // 부모-자식 관계 설정 (플레이어에 부착)
        obj.transform.SetParent(transform);

        // 적일 경우 붙었을 때 기능 정지
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnAttachedToPlayer();
        }

        // 방향 계산 (플레이어 중심 → 오브젝트 위치)
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // 플레이어 월드 반지름 계산
        float playerRadius = GetRadius();

        // 오브젝트 전체 콜라이더 범위 계산 (자식 포함)
        float objectExtent = 0f;
        Collider[] objCols = obj.GetComponentsInChildren<Collider>();
        if (objCols.Length > 0)
        {
            Bounds bounds = objCols[0].bounds;
            for (int i = 1; i < objCols.Length; i++)
                bounds.Encapsulate(objCols[i].bounds);
            objectExtent = bounds.extents.magnitude;
        }
        else
        {
            objectExtent = objectSize / 2f; // 추정치
        }

        // 최종 부착 위치 = 플레이어 중심 + (반지름 + 오브젝트 크기 + 여유) * 방향
        float extraMargin = 0.01f;
        Vector3 attachPosition = transform.position + dirFromCenter * (playerRadius + objectExtent + extraMargin);
        obj.transform.position = attachPosition;

        // 크기 성장 처리
        float growAmount = objectSize * growthRatio;
        if (obj.TryGetComponent<PickupObject>(out var pickup))
        {
            pickup.SetGrowthAmount(growAmount);
        }
        sphereCol.radius += growAmount;
    }

    /// <summary>
    /// 플레이어 반지름 감소
    /// </summary>
    public void ShrinkBy(float shrinkAmount)
    {
        sphereCol.radius -= shrinkAmount;
        sphereCol.radius = Mathf.Max(0.1f, sphereCol.radius); // 최소 보정
    }

}
