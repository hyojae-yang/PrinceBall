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
    [SerializeField] private float attachSizeRatio;

    [Header("������Ʈ ũ��� ������ ���� ����")]
    [SerializeField] private float growthRatio;

    private Rigidbody rb;
    private SphereCollider sphereCol;
    [Range(0f, 1f)]
    public float offsetTest;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();

        // ȸ�� ���� ���� (ȸ���� ���� transform���� ó��)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
            GrowBall(0.01f);
        else if (Input.GetKeyDown(KeyCode.F2))
            GrowBall(0.1f);
        else if (Input.GetKeyDown(KeyCode.F3))
            GrowBall(1f);
        // ����
        if(transform.localScale.x <= 0.1f) return; // �ʹ� �۾����� �ȵ�
        else if (Input.GetKeyDown(KeyCode.F4)) GrowBall(-0.01f);
        else if (Input.GetKeyDown(KeyCode.F5)) GrowBall(-0.1f);
        else if (Input.GetKeyDown(KeyCode.F6)) GrowBall(-1f);
#endif
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
    public float GetRadiusTEST()
    {
        if (sphereCol == null)
        {
            Debug.LogError("SphereCollider is not assigned!");
            return 0f;
        }
        //return sphereCol.radius * transform.localScale.x;
        float t = sphereCol.radius + offsetTest;

        return t * transform.localScale.x;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup))
            return;

        float playerDiameter = GetRadius() * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            // �浹 �������� �÷��̾� �߽��� ���ϴ� ���� ���� (ǥ�� ����)
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 surfaceDir = (contactPoint - transform.position).normalized;

            // ǥ�鿡 ��¦ ���̱� ���� ��������ŭ �о ��ġ ���
            Vector3 offset = transform.position + surfaceDir * GetRadiusTEST();

            AttachObject(collision.gameObject, objectSize, offset);

        }
    }
    private void AttachObject(GameObject obj, float objectSize, Vector3 tt)
    {
        // Rigidbody ��Ȱ��ȭ (���� ���� ����)
        if (obj.TryGetComponent<Rigidbody>(out var objRb))
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }
        //���� ���� ������Ʈ�� ������ġ
        obj.transform.position = tt; //�ݶ��̴��� ǥ�麤��
     
        // �θ�-�ڽ� ���� ���� (�÷��̾ ����)
        obj.transform.SetParent(transform);

        // ���� ��� �پ��� �� ��� ����
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnAttachedToPlayer();
        }

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
    
    /// <summary>
    /// �����ڿ� ���� �׽�Ʈ �޼���
    /// ���� �����ϰ� �ݶ��̴� �������� ��������ŭ ������Ŵ (���� �ƴ�)
    /// </summary>
    /// <param name="amount">���差</param>
    private void GrowBall(float amount)
    {
        // ���� ������ ��������
        Vector3 curScale = transform.localScale;

        // ��������ŭ ���ϱ�
        Vector3 newScale = curScale + new Vector3(amount, amount, amount);

        // ����
        transform.localScale = newScale;

        Debug.Log($"[GROW TEST] Scale: {curScale} �� {newScale} | Radius: {sphereCol.radius:F2}");
    }

}
