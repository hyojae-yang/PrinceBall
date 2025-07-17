using UnityEngine;

/// <summary>
/// 픽업 가능한 오브젝트에 부착하는 스크립트입니다.
/// 오브젝트의 크기 정보를 단순히 보관합니다.
/// 플레이어 스크립트에서 이 값을 참조해 성장량을 계산합니다.
/// </summary>
public class PickupObject : MonoBehaviour
{
    [Tooltip("오브젝트의 크기 값 (예: 부피 단위). 플레이어가 이 값을 참고해 성장량을 계산합니다.")]
    [SerializeField] float sizeValue;

    // 필요하면 다른 메서드 추가 가능 (예: 크기 반환)
    public float GetSizeValue()
    {
        return sizeValue;
    }
}
