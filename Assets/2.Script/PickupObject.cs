using UnityEngine;

/// <summary>
/// �Ⱦ� ������ ������Ʈ�� �����Ǵ� ������ ����ϴ�.
/// ũ�� ����, �÷��̾ ����/�и� �� �ʿ��� ���差�� ó���մϴ�.
/// </summary>
public class PickupObject : MonoBehaviour
{
    [Tooltip("������Ʈ�� ũ�� �� (��: ���� ����). �÷��̾ �� ���� ������ ���差�� ����մϴ�.")]
    [SerializeField] private float sizeValue;

    // �÷��̾ ���� �� ����� ���差 (Detach �� �÷��̾� ũ�� ���ҿ� ���)
    private float growthAmountWhenAttached;

    /// <summary>
    /// ������Ʈ ũ�� ��ȯ (���� ���� � ���)
    /// </summary>
    public float GetSizeValue() => sizeValue;

    /// <summary>
    /// �÷��̾ ���� �� ���� ���差 ����
    /// </summary>
    public void SetGrowthAmount(float amount) => growthAmountWhenAttached = amount;

    /// <summary>
    /// �÷��̾�� ������ �� �پ�� ũ�� ��ȯ
    /// </summary>
    public float GetGrowthAmount() => growthAmountWhenAttached;
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �̸��� ���
        Debug.Log($"[�浹] {gameObject.name}��'{collision.gameObject.name}' ��(��) �浹��");
    }
}
