using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour
{
    public float moveSpeed = 0.5f;                 // �⺻ �̵� �ӵ�
    float detectionRadius = 2f;             // ���� �ݰ�
    public float fleeDistance = 3f;                 // ����ġ�� �Ÿ�
    public float checkInterval = 0.5f;              // ���� ����

    [Header("��ȸ ���� ���� (���� ��ǥ ����)")]
    private Vector3 wanderAreaCenter = new Vector3(-15,0,-25); // ��ȸ ���� �߽�
    Vector2 wanderAreaSize = new Vector2(50f, 30f); // ��ȸ ���� ũ�� (X, Z)

    private Vector3 targetPosition;
    private bool isFleeing = false;
    private bool isAttached = false;             // �÷��̾�� �پ����� ����
    private Coroutine currentRoutine;

    private float myRadius;

    private void Awake()
    {
        // �ڽ��� ũ�⸦ PickupObject�κ��� ��������
        PickupObject pickup = GetComponent<PickupObject>();
        myRadius = pickup != null ? pickup.GetSizeValue() / 2f : 0.5f;

        // ���� �̺�Ʈ�� �����ϵ��� ����
        if (pickup != null)
        {
            pickup.onAttached += OnAttachedToPlayer;
        }
    }

    private void Start()
    {
        currentRoutine = StartCoroutine(WanderRoutine());
        InvokeRepeating(nameof(CheckForPlayer), 0f, checkInterval);
    }

    private void CheckForPlayer()
    {
        if (isAttached) return;  // �̹� �پ����� ����

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        float distance = Vector3.Distance(transform.position, playerObj.transform.position);

        if (distance <= detectionRadius)
        {
            PlayerController player = playerObj.GetComponent<PlayerController>();
            if (player != null && player.GetRadius() > myRadius)
            {
                if (!isFleeing)
                {
                    if (currentRoutine != null) StopCoroutine(currentRoutine);
                    isFleeing = true;
                    currentRoutine = StartCoroutine(FleeRoutine(playerObj.transform));
                }
            }
        }
        else
        {
            if (isFleeing && distance > fleeDistance)
            {
                isFleeing = false;
                if (currentRoutine != null) StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(WanderRoutine());
            }
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (!isFleeing && !isAttached)
        {
            // �簢�� ���� �� ���� ��ǥ ���� (XZ ���)
            float randomX = Random.Range(-wanderAreaSize.x * 0.5f, wanderAreaSize.x * 0.5f);
            float randomZ = Random.Range(-wanderAreaSize.y * 0.5f, wanderAreaSize.y * 0.5f);

            targetPosition = new Vector3(wanderAreaCenter.x + randomX, transform.position.y, wanderAreaCenter.z + randomZ);

            while (!isFleeing && !isAttached && Vector3.Distance(transform.position, targetPosition) > 0.2f)
            {
                Vector3 dir = (targetPosition - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private IEnumerator FleeRoutine(Transform threat)
    {
        while (isFleeing && !isAttached)
        {
            Debug.Log("�÷��̾� �����ؼ� ���� ��");
            Vector3 awayDir = (transform.position - threat.position).normalized;
            transform.position += awayDir * moveSpeed * 2f * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// �÷��̾�� �پ��� �� ȣ���
    /// </summary>
    private void OnAttachedToPlayer()
    {
        isAttached = true;
        isFleeing = false;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        CancelInvoke(nameof(CheckForPlayer)); // ���� ����
    }

    private void OnDrawGizmosSelected()
    {
        // ���� �ݰ� ���� (�����)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ��ȸ ���� �簢�� (û�ϻ�)
        Gizmos.color = Color.cyan;

        Vector3 halfSize = new Vector3(wanderAreaSize.x * 0.5f, 0, wanderAreaSize.y * 0.5f);

        Vector3 topLeft = wanderAreaCenter + new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 topRight = wanderAreaCenter + new Vector3(halfSize.x, 0, halfSize.z);
        Vector3 bottomLeft = wanderAreaCenter + new Vector3(-halfSize.x, 0, -halfSize.z);
        Vector3 bottomRight = wanderAreaCenter + new Vector3(halfSize.x, 0, -halfSize.z);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
