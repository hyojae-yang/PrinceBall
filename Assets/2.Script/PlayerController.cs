using UnityEngine;

/// <summary>
/// 플레이어 구체 이동 및 오브젝트 붙임 기능을 담당하는 스크립트입니다.
/// 실제 구체의 크기는 유지되며, 콜라이더 반경만 점점 커집니다.
/// 붙은 오브젝트의 크기에 따라 성장합니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 5f;

    [Header("붙임 크기 비율 (작을수록 큰 오브젝트가 쉽게 붙음)")]
    [SerializeField]private float attachSizeRatio;

    [Header("오브젝트 크기당 반지름 성장 비율")]
    [SerializeField]private float growthRatio;

    private Rigidbody rb;
    private SphereCollider sphereCol;

    private void Start()
    {
        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();

        // SphereCollider 가져오기
        sphereCol = GetComponent<SphereCollider>();

        // 물리 회전 방지 (회전은 transform.Rotate로 직접 처리)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 입력 방향 (로컬 → 카메라 기준으로 변환)
        Vector3 input = new Vector3(h, 0, v).normalized;

        if (input.sqrMagnitude == 0f)
            return;

        // ?? 카메라 기준 forward/right 가져오기
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Y축 제거 (수평 이동만 고려)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // ?? 카메라 기준으로 입력 방향 변환
        Vector3 moveDir = camForward * v + camRight * h;

        // 이동
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        // 회전 방향
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }


    // 오브젝트의 시각적 크기를 측정하는 함수
    // 오브젝트 전체(자식 포함)의 콜라이더 크기를 측정하는 함수
    private float GetObjectSize(GameObject obj)
    {
        float totalSize = 0f;

        // 모든 Collider 가져오기 (자식 포함)
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            Vector3 size = Vector3.zero;

            if (col is BoxCollider box)
            {
                size = Vector3.Scale(box.size, box.transform.lossyScale);
            }
            else if (col is SphereCollider sphere)
            {
                float diameter = sphere.radius * 2f * sphere.transform.lossyScale.x;
                size = new Vector3(diameter, diameter, diameter);
            }
            else if (col is CapsuleCollider capsule)
            {
                float radius = capsule.radius * capsule.transform.lossyScale.x;
                float height = capsule.height * capsule.transform.lossyScale.y;
                size = new Vector3(radius * 2f, height, radius * 2f);
            }
            else if (col is MeshCollider mesh && mesh.sharedMesh != null)
            {
                Bounds bounds = mesh.sharedMesh.bounds;
                Vector3 scaledSize = Vector3.Scale(bounds.size, mesh.transform.lossyScale);
                size = scaledSize;
            }

            float maxAxis = Mathf.Max(size.x, size.y, size.z);
            totalSize += maxAxis;
        }

        return totalSize;
    }


    private void OnCollisionEnter(Collision collision)
    {
        // "PickUp" 태그를 가진 오브젝트만 처리
        if (collision.gameObject.CompareTag("PickUp"))
        {
            float playerSize = sphereCol.radius * transform.localScale.x * 2f;
            float objectSize = GetObjectSize(collision.gameObject);

            Debug.Log($"플레이어 콜라이더 지름: {playerSize}");

            // 붙임 조건: 플레이어 크기의 1/attachSizeRatio보다 작을 경우
            if (objectSize > 0f && objectSize < playerSize / attachSizeRatio)
            {
                AttachObject(collision.gameObject, objectSize);
            }
        }
    }

    // 오브젝트를 플레이어에 붙이고 콜라이더 반경을 증가시키는 함수
    private void AttachObject(GameObject obj, float objectSize)
    {
        // Rigidbody가 있다면 물리 비활성화
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        // 부모로 붙이기
        obj.transform.SetParent(transform);
        // 플레이어 중심 방향 계산
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // 플레이어 표면에 오브젝트 위치 고정
        float playerRadius = sphereCol.radius * transform.localScale.x;
        obj.transform.position = transform.position + dirFromCenter * (playerRadius + objectSize / 2f);

        // 오브젝트 크기에 비례해 콜라이더 반경 증가
        float growAmount = objectSize * growthRatio;
        sphereCol.radius += growAmount;

        Debug.Log($"콜라이더 반경: {sphereCol.radius}");
    }
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }
}
