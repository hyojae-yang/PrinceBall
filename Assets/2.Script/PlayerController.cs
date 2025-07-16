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
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // �Է� ���� (���� �� ī�޶� �������� ��ȯ)
        Vector3 input = new Vector3(h, 0, v).normalized;

        if (input.sqrMagnitude == 0f)
            return;

        // ?? ī�޶� ���� forward/right ��������
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Y�� ���� (���� �̵��� ���)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // ?? ī�޶� �������� �Է� ���� ��ȯ
        Vector3 moveDir = camForward * v + camRight * h;

        // �̵�
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        // ȸ�� ����
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }


    // ������Ʈ�� �ð��� ũ�⸦ �����ϴ� �Լ�
    // ������Ʈ ��ü(�ڽ� ����)�� �ݶ��̴� ũ�⸦ �����ϴ� �Լ�
    private float GetObjectSize(GameObject obj)
    {
        float totalSize = 0f;

        // ��� Collider �������� (�ڽ� ����)
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
        sphereCol.radius += growAmount;

        Debug.Log($"�ݶ��̴� �ݰ�: {sphereCol.radius}");
    }
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }
}
