using UnityEngine;

/// <summary>
/// �÷��̾� ��ü �̵� �� ������Ʈ ���� ��� ���
/// ���� ��ü ũ��� �����ϸ�, �ݶ��̴� �ݰ游 ����
/// ���� ������Ʈ ũ�⿡ ���� ����
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
        rb = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();

        // ȸ�� ���� ���� (ȸ���� ���� transform���� ó��)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0, v).normalized;
        if (input.sqrMagnitude == 0f) return;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // y�� ���� �� ����ȭ�ؼ� ���� ����
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // ī�޶� ���� �Է� ���� ���
        Vector3 moveDir = camForward * v + camRight * h;
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + moveOffset);

        // �� ȸ�� �ùķ��̼� (���� ȸ�� �� ����)
        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    /// <summary>
    /// ���� �÷��̾��� ���� ������ (������ �ݿ�)
    /// </summary>
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        float playerDiameter = GetRadius() * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            AttachObject(collision.gameObject, objectSize);
        }
    }

    /// <summary>
    /// �浹�� ������Ʈ�� �÷��̾ ����
    /// </summary>
    private void AttachObject(GameObject obj, float objectSize)
    {
        // Rigidbody ��Ȱ��ȭ (���� ���� ����)
        if (obj.TryGetComponent<Rigidbody>(out var objRb))
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }

        // �θ�-�ڽ� ���� ���� (�÷��̾ ����)
        obj.transform.SetParent(transform);

        // ���� ��� �پ��� �� ��� ����
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnAttachedToPlayer();
        }

        // ���� ��� (�÷��̾� �߽� �� ������Ʈ ��ġ)
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // �÷��̾� ���� ������ ���
        float playerRadius = GetRadius();

        // ������Ʈ ��ü �ݶ��̴� ���� ��� (�ڽ� ����)
        float objectExtent = 0f;
        Collider[] objCols = obj.GetComponentsInChildren<Collider>();
        if (objCols.Length > 0)
        {
            Bounds bounds = objCols[0].bounds;
            for (int i = 1; i < objCols.Length; i++)
                bounds.Encapsulate(objCols[i].bounds);
            objectExtent = bounds.extents.magnitude;
        }
        else
        {
            objectExtent = objectSize / 2f; // ����ġ
        }

        // ���� ���� ��ġ = �÷��̾� �߽� + (������ + ������Ʈ ũ�� + ����) * ����
        float extraMargin = 0.01f;
        Vector3 attachPosition = transform.position + dirFromCenter * (playerRadius + objectExtent + extraMargin);
        obj.transform.position = attachPosition;

        // ũ�� ���� ó��
        float growAmount = objectSize * growthRatio;
        if (obj.TryGetComponent<PickupObject>(out var pickup))
        {
            pickup.SetGrowthAmount(growAmount);
        }
        sphereCol.radius += growAmount;
    }

    /// <summary>
    /// �÷��̾� ������ ����
    /// </summary>
    public void ShrinkBy(float shrinkAmount)
    {
        sphereCol.radius -= shrinkAmount;
        sphereCol.radius = Mathf.Max(0.1f, sphereCol.radius); // �ּ� ����
    }

}
