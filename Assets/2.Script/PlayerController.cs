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

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        Vector3 moveOffset = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + moveOffset);

        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir.normalized);
        float rotationAmount = 200f * Time.fixedDeltaTime;
        transform.Rotate(rotationAmount * rotateAxis, Space.World);
    }

    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        float playerDiameter = sphereCol.radius * transform.localScale.x * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            AttachObject(collision.gameObject, objectSize);
        }
    }

    private void AttachObject(GameObject obj, float objectSize)
    {
        // Rigidbody�� �ִٸ� ���� ��Ȱ��ȭ (���� �� ���� ���� ��)
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        if (objRb != null)
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }

        // �θ�-�ڽ� ���� �������� ���̱�
        obj.transform.SetParent(transform);

        // Enemy ��ũ��Ʈ�� �ִٸ� �پ��� �� ��� ���� ȣ��
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnAttachedToPlayer();
        }

        // �÷��̾� �߽� ���� ��� (���� ����)
        Vector3 dirFromCenter = (obj.transform.position - transform.position).normalized;

        // �÷��̾� ������ ��� (������ �ݿ�)
        float playerRadius = sphereCol.radius * transform.localScale.x;

        // ������Ʈ �ݶ��̴� �ջ� ��� ��� (�ڽ� ����)
        Collider[] objCols = obj.GetComponentsInChildren<Collider>();
        float objectExtent = 0f;
        if (objCols.Length > 0)
        {
            Bounds combinedBounds = objCols[0].bounds;
            for (int i = 1; i < objCols.Length; i++)
            {
                combinedBounds.Encapsulate(objCols[i].bounds);
            }
            objectExtent = combinedBounds.extents.magnitude;
        }
        else
        {
            // �ݶ��̴� ������ objectSize ���� ������ ����
            objectExtent = objectSize / 2f;
        }

        // PickupObject���� ������ �޾ƿ���
        Vector3 offset = Vector3.zero;
        if (obj.TryGetComponent<PickupObject>(out var pickup))
        {
            offset = pickup.GetAttachOffset();
        }

        // ���� ���� ��ġ ��� (�÷��̾� ǥ�� + ������Ʈ ������ + ���� + ������)
        float extraMargin = 0.01f;
        Vector3 attachPosition = transform.position + dirFromCenter * (playerRadius + objectExtent + extraMargin);
        obj.transform.position = attachPosition + obj.transform.TransformVector(offset);

        // ��ü �ݶ��̴� �ݰ� ����
        float growAmount = objectSize * growthRatio;
        if (obj.TryGetComponent<PickupObject>(out var pickup2))
        {
            pickup2.SetGrowthAmount(growAmount);
        }
        sphereCol.radius += growAmount;
    }

    public void ShrinkBy(float shrinkAmount)
    {
        sphereCol.radius -= shrinkAmount;
        sphereCol.radius = Mathf.Max(0.1f, sphereCol.radius);
    }

    public void DetachObject(GameObject obj)
    {
        if (obj.TryGetComponent<PickupObject>(out var pickup))
        {
            float shrinkAmount = pickup.GetGrowthAmount();
            ShrinkBy(shrinkAmount);

            // �θ� ����
            obj.transform.SetParent(null);

            // Rigidbody ������ ���� �ٽ� Ȱ��ȭ
            Rigidbody objRb = obj.GetComponent<Rigidbody>();
            if (objRb != null)
            {
                objRb.isKinematic = false;
                objRb.detectCollisions = true;
            }
        }
    }
}
