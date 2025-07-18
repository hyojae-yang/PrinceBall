using UnityEngine;

/// <summary>
/// �Ⱦ� ������ ������Ʈ�� �����ϴ� ��ũ��Ʈ�Դϴ�.
/// ������Ʈ�� ũ�� ������ �����ϰ�,
/// �÷��̾ �ٿ��ٰ� ����߸� �� ���差�� ����� �� �ֵ��� �մϴ�.
/// ���� ������Ʈ �߽� ��ġ ������ ���� attachOffset�� �����մϴ�.
/// </summary>
public class PickupObject : MonoBehaviour
{
    [Tooltip("������Ʈ�� ũ�� �� (��: ���� ����). �÷��̾ �� ���� ������ ���差�� ����մϴ�.")]
    [SerializeField] private float sizeValue;

    // �÷��̾ ���� �� ����� ���差 (Detach �� �÷��̾� ũ�� ���ҿ� ���)
    private float growthAmountWhenAttached;

    [Tooltip("�÷��̾ ���� �� ������ �� ������ (�⺻���� �ڵ� ���)")]
    [SerializeField] private Vector3 attachOffset = Vector3.zero;

    private void Awake()
    {
        // �������� ������ �������� ���� ��쿡�� �ڵ� ���
        if (attachOffset == Vector3.zero)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            if (colliders != null && colliders.Length > 0)
            {
                // ��� �ݶ��̴� ��踦 ���ļ� ��ü Bounds ���
                Bounds combinedBounds = colliders[0].bounds;
                for (int i = 1; i < colliders.Length; i++)
                {
                    combinedBounds.Encapsulate(colliders[i].bounds);
                }

                // ���� �߽��� ���� ��ǥ��� ��ȯ�� �� �������� ���̸� ���������� ����
                Vector3 localCenter = transform.InverseTransformPoint(combinedBounds.center);
                attachOffset = -localCenter;
            }
        }
    }

    /// <summary>
    /// ������Ʈ ũ�� ��ȯ (���� ���� � ���)
    /// </summary>
    public float GetSizeValue()
    {
        return sizeValue;
    }

    /// <summary>
    /// �÷��̾ ���� �� ���� ���差 ����
    /// </summary>
    public void SetGrowthAmount(float amount)
    {
        growthAmountWhenAttached = amount;
    }

    /// <summary>
    /// �÷��̾�� ������ �� �پ�� ũ�� ��ȯ
    /// </summary>
    public float GetGrowthAmount()
    {
        return growthAmountWhenAttached;
    }

    /// <summary>
    /// �÷��̾ ���� �� ����� ��ġ ������ ��ȯ
    /// </summary>
    public Vector3 GetAttachOffset()
    {
        return attachOffset;
    }

#if UNITY_EDITOR
    // �����Ϳ��� ������ ��ġ�� �ð������� �����ݴϴ�.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 offsetWorldPos = transform.TransformPoint(attachOffset);
        Gizmos.DrawWireSphere(offsetWorldPos, 0.05f);
        Gizmos.DrawLine(transform.position, offsetWorldPos);
    }
#endif
}
