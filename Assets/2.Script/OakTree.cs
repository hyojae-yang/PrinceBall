using UnityEngine;

/// <summary>
/// ������: ���� �ٱ� + ū �� ���� ��
/// </summary>
public class OakTree : MonoBehaviour
{
    void Start()
    {
        // �ٱ� ����
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(transform);
        trunk.transform.localPosition = Vector3.zero;
        trunk.transform.localScale = new Vector3(0.4f, 1f, 0.4f); // ���� ����
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0f); // ����

        // �� ���� (��ü)
        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaf.transform.SetParent(transform);
        leaf.transform.localPosition = new Vector3(0, 2f, 0); // �ٱ� ��
        leaf.transform.localScale = new Vector3(2f, 2f, 2f);  // ǳ���ϰ�
        leaf.GetComponent<Renderer>().material.color = new Color(0.1f, 0.8f, 0.1f); // ���ʷ�
    }
}
