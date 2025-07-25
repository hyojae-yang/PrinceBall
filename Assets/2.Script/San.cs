using UnityEngine;

/// <summary>
/// 여러 개의 큐브를 조합하여 산처럼 보이는 구조물을 생성합니다.
/// URP에서도 잘 작동하며, 경사와 높낮이를 간단히 조절할 수 있습니다.
/// </summary>
public class San : MonoBehaviour
{
    int width = 20;             // 산의 가로 큐브 수
    int depth = 20;             // 산의 세로 큐브 수
    float spacing = 2f;        // 큐브 간 거리
    float maxHeight = 10f;      // 큐브 최대 높이
    public Material mountainMaterial; // URP용 머티리얼 (없으면 회색 기본 머티리얼)

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
                // 산 중앙에 가까울수록 높아지도록 거리 계산
                float centerX = width / 2f;
                float centerZ = depth / 2f;

                float distFromCenter = Vector2.Distance(
                    new Vector2(x, z),
                    new Vector2(centerX, centerZ)
                );

                // 중심에 가까울수록 높음 (선형 감소)
                float heightFactor = Mathf.Clamp01(1f - distFromCenter / (width / 2f));
                float cubeHeight = Mathf.Lerp(1f, maxHeight, heightFactor);

                // 큐브 생성
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform; // 이 스크립트가 붙은 오브젝트 하위로

                // 크기 조정 (y축으로 키움)
                cube.transform.localScale = new Vector3(2f, cubeHeight, 2f);

                // 위치 조정 (y는 높이의 절반만큼 올려야 지면에서 시작함)
                Vector3 position = new Vector3(x * spacing, cubeHeight / 2f, z * spacing);
                cube.transform.localPosition = position;

                // 머티리얼 지정
                if (mountainMaterial != null)
                {
                    cube.GetComponent<Renderer>().material = mountainMaterial;
                }
            }
        }
    }
}
