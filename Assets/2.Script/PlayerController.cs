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

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + moveOffset);

        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        float playerDiameter = sphereCol.radius * transform.localScale.x * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            AttachObject(collision.gameObject, objectSize);
        }
    }

    private void AttachObject(GameObject obj, float objectSize)
    {
        // Rigidbody가 있다면 물리 비활성화 (붙을 때 물리 영향 끔)
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        if (objRb != null)
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }

        // 부모-자식 관계 설정으로 붙이기
        obj.transform.SetParent(transform);

        // Enemy 스크립트가 있다면 붙었을 때 기능 정지 호출
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnAttachedToPlayer();
        }

        // 플레이어 중심 방향 계산 (붙을 방향)
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // 플레이어 반지름 계산 (스케일 반영)
        float playerRadius = sphereCol.radius * transform.localScale.x;

        // 오브젝트 콜라이더 합산 경계 계산 (자식 포함)
        Collider[] objCols = obj.GetComponentsInChildren<Collider>();
        float objectExtent = 0f;
        if (objCols.Length > 0)
        {
            Bounds combinedBounds = objCols[0].bounds;
            for (int i = 1; i < objCols.Length; i++)
            {
                combinedBounds.Encapsulate(objCols[i].bounds);
            }
            objectExtent = combinedBounds.extents.magnitude;
        }
        else
        {
            // 콜라이더 없으면 objectSize 기준 반지름 추정
            objectExtent = objectSize / 2f;
        }

        // PickupObject에서 오프셋 받아오기
        Vector3 offset = Vector3.zero;
        if (obj.TryGetComponent<PickupObject>(out var pickup))
        {
            offset = pickup.GetAttachOffset();
        }

        // 최종 부착 위치 계산 (플레이어 표면 + 오브젝트 반지름 + 여유 + 오프셋)
        float extraMargin = 0.01f;
        Vector3 attachPosition = transform.position + dirFromCenter * (playerRadius + objectExtent + extraMargin);
        obj.transform.position = attachPosition + obj.transform.TransformVector(offset);

        // 구체 콜라이더 반경 증가
        float growAmount = objectSize * growthRatio;
        if (obj.TryGetComponent<PickupObject>(out var pickup2))
        {
            pickup2.SetGrowthAmount(growAmount);
        }
        sphereCol.radius += growAmount;
    }

    public void ShrinkBy(float shrinkAmount)
    {
        sphereCol.radius -= shrinkAmount;
        sphereCol.radius = Mathf.Max(0.1f, sphereCol.radius);
    }

    public void DetachObject(GameObject obj)
    {
        if (obj.TryGetComponent<PickupObject>(out var pickup))
        {
            float shrinkAmount = pickup.GetGrowthAmount();
            ShrinkBy(shrinkAmount);

            // 부모 해제
            obj.transform.SetParent(null);

            // Rigidbody 있으면 물리 다시 활성화
            Rigidbody objRb = obj.GetComponent<Rigidbody>();
            if (objRb != null)
            {
                objRb.isKinematic = false;
                objRb.detectCollisions = true;
            }
        }
    }
}
