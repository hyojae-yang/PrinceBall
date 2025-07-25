using UnityEngine;

/// <summary>
/// 소나무: 가느다란 줄기 + 위로 긴 캡슐 형태 잎
/// </summary>
public class PineTree : MonoBehaviour
{
    void Start()
    {
        // 줄기 생성
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(transform);
        trunk.transform.localPosition = Vector3.zero;
        trunk.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);  // 얇고 길게
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0f); // 갈색

        // 잎 생성 (캡슐)
        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        leaf.transform.SetParent(transform);
        leaf.transform.localPosition = new Vector3(0, 2.8f, 0); // 줄기 위
        leaf.transform.localScale = new Vector3(1f, 2f, 1f);     // 위로 길쭉
        leaf.GetComponent<Renderer>().material.color = new Color(0f, 0.6f, 0f); // 진초록
    }
}
