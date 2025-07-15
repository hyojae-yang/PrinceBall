using UnityEngine;

public class FallowCam : MonoBehaviour
{
    GameObject player;
    public Vector3 baseOffset = new Vector3(0, 1, -4);  // 기본 거리
    public float distanceScale = 1.5f; // 공 크기에 곱할 비율

    private SphereCollider playerSphereCol;

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
    }

    void LateUpdate()
    {
        if (player == null || playerSphereCol == null)
            return;

        // 플레이어 반지름 기준 거리 조정
        float playerRadius = playerSphereCol.radius * player.transform.localScale.x;

        // 기본 오프셋에 반지름 * scale 곱해서 카메라 위치 보정
        Vector3 adjustedOffset = baseOffset.normalized * (baseOffset.magnitude + playerRadius * distanceScale);

        transform.position = player.transform.position + adjustedOffset;

        // 카메라가 항상 플레이어를 바라보도록
        transform.LookAt(player.transform.position);
    }
}
