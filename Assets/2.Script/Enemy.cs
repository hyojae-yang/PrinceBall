using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// �÷��̾�� ���� ũ�� �̻��� ��� �÷��̾��� ������Ʈ�� �и���Ű�� ���ʹ�
/// + �ڵ� �պ� �̵�
/// + PickupObject �и� ��� ����
/// + �÷��̾� �浹 �� ƨ���� ������ �� ���� (������ٵ� ���� ���)
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("�÷��̾�� �󸶳� Ŀ�� �����ϴ°�")]
    public float sizeThresholdMultiplier = 1.2f;

    [Header("�ڵ� �̵� ����")]
    public Vector3 moveDirection = Vector3.forward;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    [Header("�и� ��ٿ� ����")]
    public float dropCooldown = 1f;

    [Header("�浹 ƨ�ܳ��� �� ����")]
    public float bounceForce;

    private Vector3 startPos;
    private int moveDirSign = 1;

    private PickupObject pickup;
    private bool hasRecentlyTriggered = false;

    // �÷��̾ �پ����� ����
    private bool isAttached = false;

    private void Awake()
    {
        pickup = GetComponent<PickupObject>();
        startPos = transform.position;
    }

    private void Update()
    {
        if (isAttached)
            return;

        MoveEnemy();
    }

    /// <summary>
    /// ������ �պ� �̵�
    /// </summary>
    private void MoveEnemy()
    {
        Vector3 dirNormalized = moveDirection.normalized;
        Vector3 offset = dirNormalized * moveSpeed * moveDirSign * Time.deltaTime;
        transform.position += offset;

        Vector3 toCurrent = transform.position - startPos;
        float projectedDistance = Vector3.Dot(toCurrent, dirNormalized);

        if (projectedDistance > moveDistance)
        {
            moveDirSign = -1;
            transform.position = startPos + dirNormalized * moveDistance;
            transform.rotation = Quaternion.Euler(0f, 0f, 270f);
        }
        else if (projectedDistance < 0f)
        {
            moveDirSign = 1;
            transform.position = startPos;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasRecentlyTriggered || isAttached)
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        float playerRadius = player.GetRadius();
        float myRadius = pickup.GetSizeValue() / 2f;

        if (myRadius > playerRadius * sizeThresholdMultiplier)
        {
            Debug.Log("�÷��̾�� ũ�Ƿ� �и� ���� ����!");

            // DropFromPlayer�� List<Transform> ��ȯ�ϵ��� ���� ��,
            List<Transform> droppedObjects = DropFromPlayer(player);

            // �и��� ������Ʈ�� ƨ�ܳ���
            BounceObjects(player, droppedObjects);

            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
        else
        {
            Debug.Log("�÷��̾�� �۾Ƽ� ���õ�");
        }
    }


    /// <summary>
    /// �÷��̾ ���� ������Ʈ�� �� �Ϻθ� �����ϰ� ���
    /// </summary>
    private List<Transform> DropFromPlayer(PlayerController player)
    {
        GameObject playerObj = player.gameObject;
        List<PickupObject> pickups = new List<PickupObject>();

        // �÷��̾� �ڽ� �� PickupObject ������Ʈ�� �ִ� �͵��� ����
        foreach (Transform child in playerObj.transform)
        {
            var pickup = child.GetComponent<PickupObject>();
            if (pickup != null)
                pickups.Add(pickup);
        }

        if (pickups.Count == 0)
        {
            return new List<Transform>(); // �и��� �� ������ �� ����Ʈ ��ȯ
        }

        int dropCount = Mathf.Min(5, pickups.Count);
        var randomPickups = pickups.OrderBy(x => Random.value).Take(dropCount).ToList();

        float totalShrinkAmount = 0f;
        List<Transform> droppedObjects = new List<Transform>(); // �и��� ������Ʈ ����Ʈ

        foreach (var pickup in randomPickups)
        {
            Transform obj = pickup.transform;
            obj.parent = null;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                                            // ƨ��� ���� ���⼭ �����մϴ�
            }

            totalShrinkAmount += pickup.GetGrowthAmount();
            droppedObjects.Add(obj);

            Debug.Log($"���� �и���: {obj.name}");
        }

        if (totalShrinkAmount > 0f)
        {
            player.ShrinkBy(totalShrinkAmount);
            Debug.Log($"�÷��̾� ũ�� ����: {totalShrinkAmount}");
        }

        return droppedObjects; // �и��� ������Ʈ ����Ʈ ��ȯ
    }


    /// <summary>
    /// �÷��̾�� �ε�ģ �� ƨ���� ������ �ϴ� �Լ�
    /// ������ٵ� ������ ��ġ�� ��¦ �о�� ��� ����
    /// </summary>
    private void BounceObjects(PlayerController player, List<Transform> droppedObjects)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        float scaledBounceForce = bounceForce * player.GetRadius();
        // �÷��̺� ƨ���� ���⿡ Y�� ���� ���� �߰� (0.5f ���� �÷��ֱ�)
        Vector3 bounceDir = (player.transform.position - transform.position).normalized + Vector3.up * 0.5f;
        bounceDir.Normalize();

        // �÷��̾� �� ƨ�ܳ���
        if (playerRb != null)
        {
            playerRb.AddForce(bounceDir * scaledBounceForce, ForceMode.Impulse);
        }
        else
        {
            player.transform.position += bounceDir * 0.5f;
        }

        foreach (var objTransform in droppedObjects)
        {
            Rigidbody rb = objTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // �÷��̾� �߽��� �������� ���� ���
                Vector3 baseDir = (objTransform.position - player.transform.position).normalized;

                // ���� Ȯ�� �߰� (�� �ڿ�������)
                Vector3 randomSpread = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(0.6f, 1.0f),  // ���� ƨ��� ���� ����
                    Random.Range(-0.3f, 0.3f)
                );

                Vector3 forceDir = (baseDir + randomSpread).normalized;

                rb.isKinematic = false;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                // ��¦ ����� ���� ����
                objTransform.position += Vector3.up * 0.2f;

                // �����ϸ��� �� ����
                rb.AddForce(forceDir * scaledBounceForce, ForceMode.Impulse);
            }
            else
            {
                // ������ٵ� ���� ��쵵 ����� �������� ���ϰ� �б�
                Vector3 fallbackDir = (objTransform.position - player.transform.position).normalized + Vector3.up * 0.5f;
                objTransform.position += fallbackDir.normalized * 0.3f;
            }
        }

    }



    private void ResetTrigger()
    {
        hasRecentlyTriggered = false;
    }

    /// <summary>
    /// �÷��̾ ���� �� ȣ��Ǵ� �޼��� (��� ������)
    /// </summary>
    public void OnAttachedToPlayer()
    {
        isAttached = true;
        StopAllCoroutines();
    }
}
