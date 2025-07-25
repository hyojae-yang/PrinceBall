using UnityEngine;

/// <summary>
/// 참나무: 굵은 줄기 + 큰 구 형태 잎
/// </summary>
public class OakTree : MonoBehaviour
{
    void Start()
    {
        // 줄기 생성
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(transform);
        trunk.transform.localPosition = Vector3.zero;
        trunk.transform.localScale = new Vector3(0.4f, 1f, 0.4f); // 굵고 낮게
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0f); // 갈색

        // 잎 생성 (구체)
        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaf.transform.SetParent(transform);
        leaf.transform.localPosition = new Vector3(0, 2f, 0); // 줄기 위
        leaf.transform.localScale = new Vector3(2f, 2f, 2f);  // 풍성하게
        leaf.GetComponent<Renderer>().material.color = new Color(0.1f, 0.8f, 0.1f); // 연초록
    }
}
