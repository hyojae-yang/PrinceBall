using UnityEngine;

/// <summary>
/// 픽업 가능한 오브젝트에 부착하는 스크립트입니다.
/// 오브젝트의 크기 정보를 보관하고,
/// 플레이어가 붙였다가 떨어뜨릴 때 성장량을 계산할 수 있도록 합니다.
/// 또한 오브젝트 중심 위치 보정을 위한 attachOffset을 제공합니다.
/// </summary>
public class PickupObject : MonoBehaviour
{
    [Tooltip("오브젝트의 크기 값 (예: 부피 단위). 플레이어가 이 값을 참고해 성장량을 계산합니다.")]
    [SerializeField] private float sizeValue;

    // 플레이어에 붙을 때 적용된 성장량 (Detach 시 플레이어 크기 감소에 사용)
    private float growthAmountWhenAttached;

    [Tooltip("플레이어에 붙을 때 기준이 될 오프셋 (기본값은 자동 계산)")]
    [SerializeField] private Vector3 attachOffset = Vector3.zero;

    private void Awake()
    {
        // 수동으로 오프셋 지정하지 않은 경우에만 자동 계산
        if (attachOffset == Vector3.zero)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            if (colliders != null && colliders.Length > 0)
            {
                // 모든 콜라이더 경계를 합쳐서 전체 Bounds 계산
                Bounds combinedBounds = colliders[0].bounds;
                for (int i = 1; i < colliders.Length; i++)
                {
                    combinedBounds.Encapsulate(colliders[i].bounds);
                }

                // 월드 중심을 로컬 좌표계로 변환한 후 원점과의 차이를 오프셋으로 저장
                Vector3 localCenter = transform.InverseTransformPoint(combinedBounds.center);
                attachOffset = -localCenter;
            }
        }
    }

    /// <summary>
    /// 오브젝트 크기 반환 (부착 조건 등에 사용)
    /// </summary>
    public float GetSizeValue()
    {
        return sizeValue;
    }

    /// <summary>
    /// 플레이어에 붙을 때 계산된 성장량 저장
    /// </summary>
    public void SetGrowthAmount(float amount)
    {
        growthAmountWhenAttached = amount;
    }

    /// <summary>
    /// 플레이어에서 떨어질 때 줄어들 크기 반환
    /// </summary>
    public float GetGrowthAmount()
    {
        return growthAmountWhenAttached;
    }

    /// <summary>
    /// 플레이어에 붙일 때 사용할 위치 오프셋 반환
    /// </summary>
    public Vector3 GetAttachOffset()
    {
        return attachOffset;
    }

#if UNITY_EDITOR
    // 에디터에서 오프셋 위치를 시각적으로 보여줍니다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 offsetWorldPos = transform.TransformPoint(attachOffset);
        Gizmos.DrawWireSphere(offsetWorldPos, 0.05f);
        Gizmos.DrawLine(transform.position, offsetWorldPos);
    }
#endif
}
