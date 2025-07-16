using UnityEngine;

public class FallowCam : MonoBehaviour
{
    GameObject player;
    public Vector3 baseOffset = new Vector3(0, 0.5f, -0.5f);  // �⺻ �Ÿ�
    public float distanceScale = 1.5f; // �� ũ�⿡ ���� ����

    public float rotationSpeed = 5f; // ���콺 �巡�� �� ȸ�� �ӵ�
    public float minVerticalAngle = -30f; // ī�޶� ���� �ּ� ���� ����
    public float maxVerticalAngle = 60f;  // ī�޶� ���� �ִ� ���� ����

    private SphereCollider playerSphereCol;

    private float yaw = 0f;   // ���� ȸ�� ���� (y��)
    private float pitch = 20f; // ���� ȸ�� ���� (x��), �⺻�� �ణ �����ٺ��� ����

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

        // �ʱ� yaw, pitch ���� (�÷��̾� �ٶ󺸴� �⺻ ��������)
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (player == null || playerSphereCol == null)
            return;

        // ���콺 ��Ŭ���� ���� ���¿��� �巡�� �� ī�޶� ȸ�� ����
        if (Input.GetMouseButton(0))
        {
            // ���콺 ������ �Է� �ޱ�
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // ���콺 �����ӿ� ���� ȸ�� ���� ���� (�ӵ� ���� ����)
            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * rotationSpeed;

            // ���� ���� ����
            pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
        }

        // �÷��̾� ������ ���� �Ÿ� ���
        float playerRadius = playerSphereCol.radius * player.transform.localScale.x;

        // ȸ�� ������ ���ʹϾ����� ��ȯ
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // �⺻ ������ ������ ũ�⸦ �����ؼ� �Ÿ� ����
        float distance = baseOffset.magnitude + playerRadius * distanceScale;

        // ȸ�� ������ ��ġ ��� (�÷��̾� �ֺ�)
        Vector3 offset = rotation * (Vector3.forward * -distance);

        // ī�޶� ��ġ ����
        transform.position = player.transform.position + offset;

        // ī�޶� �÷��̾ �ٶ󺸵��� ����
        transform.LookAt(player.transform.position);
    }
}
