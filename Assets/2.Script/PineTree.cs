using UnityEngine;

/// <summary>
/// �ҳ���: �����ٶ� �ٱ� + ���� �� ĸ�� ���� ��
/// </summary>
public class PineTree : MonoBehaviour
{
    void Start()
    {
        // �ٱ� ����
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(transform);
        trunk.transform.localPosition = Vector3.zero;
        trunk.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);  // ��� ���
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0f); // ����

        // �� ���� (ĸ��)
        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        leaf.transform.SetParent(transform);
        leaf.transform.localPosition = new Vector3(0, 2.8f, 0); // �ٱ� ��
        leaf.transform.localScale = new Vector3(1f, 2f, 1f);     // ���� ����
        leaf.GetComponent<Renderer>().material.color = new Color(0f, 0.6f, 0f); // ���ʷ�
    }
}
