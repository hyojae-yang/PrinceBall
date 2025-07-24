using UnityEngine;

/// <summary>
/// Dog ���ʹ� AI: �÷��̾ ���� �þ߰��� �Ÿ� �ȿ��� �����ϸ� �����ϰ�, ��ġ�� ���� �ڸ��� ���ư��ϴ�.
/// </summary>
public class Dog : Enemy
{
    [Header("�÷��̾� ���� ����")]
    public float viewAngle = 90f;             // �þ߰� (��)
    public float viewDistance = 5f;           // �þ� �Ÿ�
    public float moveSpeed = 3f;              // ���� �̵� �ӵ�
    private float stopDistance = 0.1f;        // ���� �Ϸ� ������ �Ÿ�

    private Transform player;                 // �÷��̾� Ʈ������
    private Vector3 startPos;                 // ���װ� �¾ ��ġ
    private Quaternion startRotation; // ���װ� �¾ ����
    private bool isReturning = false;         // ���� ������ ����

    /// <summary>
    /// �ʱ�ȭ
    /// </summary>
    protected new void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    /// <summary>
    /// �� �����Ӹ��� ȣ��
    /// </summary>
    void Update()
    {
        if (isAttached || player == null)
            return;

        if (IsPlayerInSight())
        {
            isReturning = false;
            MoveToPlayer();
        }
        else
        {
            ReturnToStart();
        }
    }

    /// <summary>
    /// �÷��̾ �þ߰��� �Ÿ� ���� �ִ����� �Ǵ�
    /// </summary>
    bool IsPlayerInSight()
    {
        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // 1. �Ÿ� üũ
        if (distanceToPlayer > viewDistance)
            return false;

        // 2. �þ߰� üũ
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > viewAngle * 0.5f)
            return false;
        

        // 3. ��ֹ� üũ (����ĳ��Ʈ)
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, toPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, viewDistance))
        {
            if (hit.transform != player)
            {
                // ����ĳ��Ʈ�� �÷��̾�� ���� �ٸ� ������Ʈ�� �¾��� �� ��������
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// �÷��̾� ������ �̵�
    /// </summary>
    void MoveToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // �ڿ������� ȸ��
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }

    /// <summary>
    /// ���� ��ġ�� ����
    /// </summary>
    void ReturnToStart()
    {
        if (!isReturning)
            isReturning = true;

        Vector3 toStart = startPos - transform.position;
        float distance = toStart.magnitude;

        if (distance < stopDistance)
        {
            isReturning = false;
            transform.rotation = startRotation; // ���� ����
            return;
        }

        Vector3 dir = toStart.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // �ڿ������� ȸ��
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Scene �信�� �þ� ���� ����� ǥ��
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance); // ��ü Ž�� �Ÿ�

        // ��ä�� �þ� �ð�ȭ
        Vector3 forward = transform.forward;
        Quaternion leftRot = Quaternion.Euler(0, -viewAngle / 2f, 0);
        Quaternion rightRot = Quaternion.Euler(0, viewAngle / 2f, 0);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * viewDistance);
    }
#endif
}
