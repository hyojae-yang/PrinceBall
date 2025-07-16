using UnityEngine;

public class FallowCam : MonoBehaviour
{
    GameObject player;
    public Vector3 baseOffset = new Vector3(0, 0.5f, -0.5f);  // 기본 거리
    public float distanceScale = 1.5f; // 공 크기에 곱할 비율

    public float rotationSpeed = 5f; // 마우스 드래그 시 회전 속도
    public float minVerticalAngle = -30f; // 카메라 수직 최소 각도 제한
    public float maxVerticalAngle = 60f;  // 카메라 수직 최대 각도 제한

    private SphereCollider playerSphereCol;

    private float yaw = 0f;   // 수평 회전 각도 (y축)
    private float pitch = 20f; // 수직 회전 각도 (x축), 기본값 약간 내려다보는 시점

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerSphereCol = player.GetComponent<SphereCollider>();
            if (playerSphereCol == null)
            {
                Debug.LogError("플레이어에 SphereCollider가 없습니다!");
            }
        }

        // 초기 yaw, pitch 설정 (플레이어 바라보는 기본 방향으로)
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (player == null || playerSphereCol == null)
            return;

        // 마우스 좌클릭을 누른 상태에서 드래그 시 카메라 회전 조절
        if (Input.GetMouseButton(0))
        {
            // 마우스 움직임 입력 받기
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // 마우스 움직임에 따라 회전 각도 변경 (속도 조절 포함)
            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * rotationSpeed;

            // 수직 각도 제한
            pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
        }

        // 플레이어 반지름 기준 거리 계산
        float playerRadius = playerSphereCol.radius * player.transform.localScale.x;

        // 회전 각도를 쿼터니언으로 변환
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // 기본 오프셋 벡터의 크기를 조절해서 거리 설정
        float distance = baseOffset.magnitude + playerRadius * distanceScale;

        // 회전 적용한 위치 계산 (플레이어 주변)
        Vector3 offset = rotation * (Vector3.forward * -distance);

        // 카메라 위치 지정
        transform.position = player.transform.position + offset;

        // 카메라가 플레이어를 바라보도록 설정
        transform.LookAt(player.transform.position);
    }
}
