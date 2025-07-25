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
    [SerializeField] private float attachSizeRatio;

    [Header("오브젝트 크기당 반지름 성장 비율")]
    [SerializeField] private float growthRatio;

    private Rigidbody rb;
    private SphereCollider sphereCol;
    [Range(0f, 1f)]
    public float offsetTest;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();

        // 회전 물리 제한 (회전은 직접 transform으로 처리)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
            GrowBall(0.01f);
        else if (Input.GetKeyDown(KeyCode.F2))
            GrowBall(0.1f);
        else if (Input.GetKeyDown(KeyCode.F3))
            GrowBall(1f);
        // 감소
        if(transform.localScale.x <= 0.1f) return; // 너무 작아지면 안됨
        else if (Input.GetKeyDown(KeyCode.F4)) GrowBall(-0.01f);
        else if (Input.GetKeyDown(KeyCode.F5)) GrowBall(-0.1f);
        else if (Input.GetKeyDown(KeyCode.F6)) GrowBall(-1f);
#endif
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
    public float GetRadiusTEST()
    {
        if (sphereCol == null)
        {
            Debug.LogError("SphereCollider is not assigned!");
            return 0f;
        }
        //return sphereCol.radius * transform.localScale.x;
        float t = sphereCol.radius + offsetTest;

        return t * transform.localScale.x;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        float playerDiameter = GetRadius() * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            // 충돌 지점에서 플레이어 중심을 향하는 방향 벡터 (표면 방향)
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 surfaceDir = (contactPoint - transform.position).normalized;

            // 표면에 살짝 붙이기 위해 반지름만큼 밀어낸 위치 계산
            Vector3 offset = transform.position + surfaceDir * GetRadiusTEST();

            AttachObject(collision.gameObject, objectSize, offset);

        }
    }
    private void AttachObject(GameObject obj, float objectSize, Vector3 tt)
    {
        // Rigidbody 비활성화 (물리 영향 제거)
        if (obj.TryGetComponent<Rigidbody>(out var objRb))
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }
        //실제 붙은 오브젝트의 월드위치
        obj.transform.position = tt; //콜라이더의 표면벡터
     
        // 부모-자식 관계 설정 (플레이어에 부착)
        obj.transform.SetParent(transform);

        // 적일 경우 붙었을 때 기능 정지
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnAttachedToPlayer();
        }

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
    
    /// <summary>
    /// 개발자용 성장 테스트 메서드
    /// 로컬 스케일과 콜라이더 반지름을 고정값만큼 증가시킴 (곱셈 아님)
    /// </summary>
    /// <param name="amount">성장량</param>
    private void GrowBall(float amount)
    {
        // 현재 스케일 가져오기
        Vector3 curScale = transform.localScale;

        // 고정량만큼 더하기
        Vector3 newScale = curScale + new Vector3(amount, amount, amount);

        // 적용
        transform.localScale = newScale;

        Debug.Log($"[GROW TEST] Scale: {curScale} → {newScale} | Radius: {sphereCol.radius:F2}");
    }

}
