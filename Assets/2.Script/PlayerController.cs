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
    [SerializeField]private float attachSizeRatio;

    [Header("������Ʈ ũ��� ������ ���� ����")]
    [SerializeField]private float growthRatio;

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
        // �Է� �� �ޱ�
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.sqrMagnitude == 0f)
            return;

        // ��ġ �̵�
        Vector3 moveOffset = inputDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        // ȸ�� ó�� (�Է� ���⿡ ���� ȸ���� ���)
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, inputDir);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    // ������Ʈ�� �ð��� ũ�⸦ �����ϴ� �Լ�
    private float GetObjectSize(GameObject obj)
    {
        SphereCollider sphere = obj.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            return sphere.radius * obj.transform.localScale.x * 2f; // ����
        }

        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box != null)
        {
            Vector3 size = Vector3.Scale(box.size, obj.transform.localScale);
            return Mathf.Max(size.x, size.y, size.z); // ���� �� �� ����
        }

        return 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // "PickUp" �±׸� ���� ������Ʈ�� ó��
        if (collision.gameObject.CompareTag("PickUp"))
        {
            float playerSize = sphereCol.radius * transform.localScale.x * 2f;
            float objectSize = GetObjectSize(collision.gameObject);

            Debug.Log($"�÷��̾� �ݶ��̴� ����: {playerSize}");

            // ���� ����: �÷��̾� ũ���� 1/attachSizeRatio���� ���� ���
            if (objectSize > 0f && objectSize < playerSize / attachSizeRatio)
            {
                AttachObject(collision.gameObject, objectSize);
            }
        }
    }

    // ������Ʈ�� �÷��̾ ���̰� �ݶ��̴� �ݰ��� ������Ű�� �Լ�
    private void AttachObject(GameObject obj, float objectSize)
    {
        // �θ�� ���̱�
        obj.transform.SetParent(transform);

        // �÷��̾� �߽� ���� ���
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // �÷��̾� ǥ�鿡 ������Ʈ ��ġ ����
        float playerRadius = sphereCol.radius * transform.localScale.x;
        obj.transform.position = transform.position + dirFromCenter * (playerRadius + objectSize / 2f);

        // ������Ʈ ũ�⿡ ����� �ݶ��̴� �ݰ� ����
        float growAmount = objectSize * growthRatio;
        sphereCol.radius += growAmount;

        Debug.Log($"�ݶ��̴� �ݰ�: {sphereCol.radius}");
    }
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }
}
