using UnityEngine;

/// <summary>
/// �Ⱦ� ������ ������Ʈ�� �����ϴ� ��ũ��Ʈ�Դϴ�.
/// ������Ʈ�� ũ�� ������ �ܼ��� �����մϴ�.
/// �÷��̾� ��ũ��Ʈ���� �� ���� ������ ���差�� ����մϴ�.
/// </summary>
public class PickupObject : MonoBehaviour
{
    [Tooltip("������Ʈ�� ũ�� �� (��: ���� ����). �÷��̾ �� ���� ������ ���差�� ����մϴ�.")]
    [SerializeField] float sizeValue;

    // �ʿ��ϸ� �ٸ� �޼��� �߰� ���� (��: ũ�� ��ȯ)
    public float GetSizeValue()
    {
        return sizeValue;
    }
}
