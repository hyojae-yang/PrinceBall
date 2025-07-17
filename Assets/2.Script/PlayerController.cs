using UnityEngine;

/// <summary>
/// �÷��̾� ��ü �̵� �� ������Ʈ ���� ����� ����ϴ� ��ũ��Ʈ�Դϴ�.
/// ���� ��ü�� ũ��� �����Ǹ�, �ݶ��̴� �ݰ游 ���� Ŀ���ϴ�.
/// ���� ������Ʈ�� ũ�⿡ ���� �����մϴ�.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("�̵� �ӵ�")]
    public float moveSpeed = 5f;

    [Header("���� ũ�� ���� (�������� ū ������Ʈ�� ���� ����)")]
    [SerializeField] private float attachSizeRatio = 1.5f;

    [Header("������Ʈ ũ��� ������ ���� ����")]
    [SerializeField] private float growthRatio = 0.04f;

    private Rigidbody rb;
    private SphereCollider sphereCol;

    private void Start()
    {
        // Rigidbody ������Ʈ ��������
        rb = GetComponent<Rigidbody>();

        // SphereCollider ��������
        sphereCol = GetComponent<SphereCollider>();

        // ���� ȸ�� ���� (ȸ���� transform.Rotate�� ���� ó��)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // �Է� ���� (���� �� ī�޶� �������� ��ȯ)
        Vector3 input = new Vector3(h, 0, v).normalized;

        if (input.sqrMagnitude == 0f)
            return;

        // ī�޶� ���� forward/right ��������
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Y�� ���� (���� �̵��� ���)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // ī�޶� �������� �Է� ���� ��ȯ
        Vector3 moveDir = camForward * v + camRight * h;

        // �̵�
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        // ȸ�� ����
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    // �÷��̾��� ���� ���� ������ ��ȯ (localScale ���)
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // PickupObject ��ũ��Ʈ�� �پ��ִ� ������Ʈ�� ó��
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        // �÷��̾��� ���� ���� ���
        float playerDiameter = sphereCol.radius * transform.localScale.x * 2f;

        // �Ⱦ� ������Ʈ�� ũ�� ��
        float objectSize = pickup.GetSizeValue();

        // ���� ����: �÷��̾� ���� / �������� ������Ʈ�� �۾ƾ� ��
        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            AttachObject(collision.gameObject, objectSize);
        }
    }


    // ������Ʈ�� �÷��̾ ���̰� �ݶ��̴� �ݰ��� ������Ű�� �Լ�
    private void AttachObject(GameObject obj, float objectSize)
    {
        // Rigidbody�� �ִٸ� ���� ��Ȱ��ȭ
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // �θ�� ���̱�
        obj.transform.SetParent(transform);

        // �÷��̾� �߽� ���� ���
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // �÷��̾� ǥ�鿡 ������Ʈ ��ġ ����
        float playerRadius = sphereCol.radius * transform.localScale.x;
        obj.transform.position = transform.position + dirFromCenter * (playerRadius + objectSize / 2f);

        // ������Ʈ ũ�⿡ ����� �ݶ��̴� �ݰ� ����
        float growAmount = objectSize * growthRatio;

        // �ݶ��̴� �ݰ� ����
        sphereCol.radius += growAmount;
    }
}
