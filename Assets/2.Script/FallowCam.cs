using UnityEngine;

public class FallowCam : MonoBehaviour
{
    GameObject player;
    public Vector3 baseOffset = new Vector3(0, 1, -4);  // �⺻ �Ÿ�
    public float distanceScale = 1.5f; // �� ũ�⿡ ���� ����

    private SphereCollider playerSphereCol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerSphereCol = player.GetComponent<SphereCollider>();
            if (playerSphereCol == null)
            {
                Debug.LogError("�÷��̾ SphereCollider�� �����ϴ�!");
            }
        }
    }

    void LateUpdate()
    {
        if (player == null || playerSphereCol == null)
            return;

        // �÷��̾� ������ ���� �Ÿ� ����
        float playerRadius = playerSphereCol.radius * player.transform.localScale.x;

        // �⺻ �����¿� ������ * scale ���ؼ� ī�޶� ��ġ ����
        Vector3 adjustedOffset = baseOffset.normalized * (baseOffset.magnitude + playerRadius * distanceScale);

        transform.position = player.transform.position + adjustedOffset;

        // ī�޶� �׻� �÷��̾ �ٶ󺸵���
        transform.LookAt(player.transform.position);
    }
}
