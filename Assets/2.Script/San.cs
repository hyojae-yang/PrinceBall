using UnityEngine;

/// <summary>
/// ���� ���� ť�긦 �����Ͽ� ��ó�� ���̴� �������� �����մϴ�.
/// URP������ �� �۵��ϸ�, ���� �����̸� ������ ������ �� �ֽ��ϴ�.
/// </summary>
public class San : MonoBehaviour
{
    int width = 20;             // ���� ���� ť�� ��
    int depth = 20;             // ���� ���� ť�� ��
    float spacing = 2f;        // ť�� �� �Ÿ�
    float maxHeight = 10f;      // ť�� �ִ� ����
    public Material mountainMaterial; // URP�� ��Ƽ���� (������ ȸ�� �⺻ ��Ƽ����)

    void Start()
    {
        GenerateMountain();
    }

    void GenerateMountain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // �� �߾ӿ� �������� ���������� �Ÿ� ���
                float centerX = width / 2f;
                float centerZ = depth / 2f;

                float distFromCenter = Vector2.Distance(
                    new Vector2(x, z),
                    new Vector2(centerX, centerZ)
                );

                // �߽ɿ� �������� ���� (���� ����)
                float heightFactor = Mathf.Clamp01(1f - distFromCenter / (width / 2f));
                float cubeHeight = Mathf.Lerp(1f, maxHeight, heightFactor);

                // ť�� ����
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform; // �� ��ũ��Ʈ�� ���� ������Ʈ ������

                // ũ�� ���� (y������ Ű��)
                cube.transform.localScale = new Vector3(2f, cubeHeight, 2f);

                // ��ġ ���� (y�� ������ ���ݸ�ŭ �÷��� ���鿡�� ������)
                Vector3 position = new Vector3(x * spacing, cubeHeight / 2f, z * spacing);
                cube.transform.localPosition = position;

                // ��Ƽ���� ����
                if (mountainMaterial != null)
                {
                    cube.GetComponent<Renderer>().material = mountainMaterial;
                }
            }
        }
    }
}
