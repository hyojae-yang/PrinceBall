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
        // 입력 값 받기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.sqrMagnitude == 0f)
            return;

        // 위치 이동
        Vector3 moveOffset = inputDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        // 회전 처리 (입력 방향에 따라 회전축 계산)
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, inputDir);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    // 오브젝트의 시각적 크기를 측정하는 함수
    private float GetObjectSize(GameObject obj)
    {
        SphereCollider sphere = obj.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            return sphere.radius * obj.transform.localScale.x * 2f; // 지름
        }

        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box != null)
        {
            Vector3 size = Vector3.Scale(box.size, obj.transform.localScale);
            return Mathf.Max(size.x, size.y, size.z); // 가장 긴 축 기준
        }

        return 0f;
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
