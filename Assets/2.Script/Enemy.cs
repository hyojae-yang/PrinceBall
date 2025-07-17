using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// �÷��̾�� �浹 ��, �÷��̾�� ���� ũ�� �̻��̸� �����ϴ� '�и���' ������ ������Ʈ
/// + ������ �պ� �ڵ� �̵� ��� ����
/// + PickupObject �и� ��� ����
/// </summary>
[RequireComponent(typeof(PickupObject))]
public class Enemy : MonoBehaviour
{
    [Header("�÷��̾�� �󸶳� Ŀ�� �����ϴ°�")]
    public float sizeThresholdMultiplier = 1.2f; // �÷��̾�� �� �� �̻� ũ�� ��������

    [Header("�ڵ� �̵� ����")]
    public Vector3 moveDirection = Vector3.forward; // ���� ���� �̵� ����
    public float moveDistance = 3f;                 // �̵� �Ÿ� (�� ���� �ִ� �Ÿ�)
    public float moveSpeed = 2f;                    // �̵� �ӵ� (m/s)

    [Header("�и� ��ٿ� ����")]
    public float dropCooldown = 1f;                 // �и� �� �� �ʰ� �ٽ� �и� ���ϰ� ������

    private Vector3 startPos;
    private int moveDirSign = 1;

    private PickupObject pickup;
    private bool hasRecentlyTriggered = false;

    private void Awake()
    {
        pickup = GetComponent<PickupObject>();
        startPos = transform.position;
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// ������ �պ� �̵�
    /// </summary>
    private void MoveEnemy()
    {
        Vector3 dirNormalized = moveDirection.normalized;

        // �̵� ���� * �ӵ� * �ð� * ��ȣ
        Vector3 offset = dirNormalized * moveSpeed * moveDirSign * Time.deltaTime;
        transform.position += offset;

        Vector3 toCurrent = transform.position - startPos;
        float projectedDistance = Vector3.Dot(toCurrent, dirNormalized);

        if (projectedDistance > moveDistance)
        {
            moveDirSign = -1;
            transform.position = startPos + dirNormalized * moveDistance;
            transform.rotation = Quaternion.Euler(0f, 0f, 270f); // �ݴ����
        }
        else if (projectedDistance < 0f)
        {
            moveDirSign = 1;
            transform.position = startPos;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);  // ���� ����
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasRecentlyTriggered)
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        float playerRadius = player.GetRadius();
        float myRadius = pickup.GetSizeValue() / 2f;

        if (myRadius > playerRadius * sizeThresholdMultiplier)
        {
            Debug.Log("�÷��̾�� ũ�Ƿ� �и� ���� ����!");

            // �и� ��� ����
            DropFromPlayer(player.gameObject);

            // �ߺ� ���� ��ٿ�
            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
        else
        {
            Debug.Log("�÷��̾�� �۾Ƽ� ���õ�");
        }
    }

/// <summary>
/// �÷��̾ ���� �ڽ� ������Ʈ �� �Ϻθ� �����ϰ� ������ϴ�.
/// </summary>
private void DropFromPlayer(GameObject playerObj)
{
    // �÷��̾��� �ڽ� �� PickupObject ������Ʈ�� ���� ������Ʈ�� ����
    List<PickupObject> pickups = new List<PickupObject>();

    foreach (Transform child in playerObj.transform)
    {
        var pickup = child.GetComponent<PickupObject>();
        if (pickup != null)
            pickups.Add(pickup);
    }

    if (pickups.Count == 0)
    {
        Debug.Log("�и��� ������Ʈ�� ����");
        return;
    }

    // �и��� ���� (��: 3������)
    int dropCount = Mathf.Min(5, pickups.Count);

    // ����Ʈ�� �����ϰ� ����, �Ϻθ� ����
    var randomPickups = pickups.OrderBy(x => Random.value).Take(dropCount).ToList();

    foreach (var pickup in randomPickups)
    {
        Transform obj = pickup.transform;

        obj.parent = null; // �θ𿡼� �и�

        // ������ٵ� ���� Ȱ��ȭ
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        Debug.Log($"���� �и���: {obj.name}");
    }
}


/// <summary>
/// ���� �ð� �� �ٽ� ���� �����ϰ� �ʱ�ȭ
/// </summary>
private void ResetTrigger()
    {
        hasRecentlyTriggered = false;
    }
}
