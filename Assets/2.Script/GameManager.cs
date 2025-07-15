using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("목표 크기 (지름 기준)")]
    public float targetDiameter = 10f;

    [Header("제한 시간 (초)")]
    public float totalTime = 60f;

    private float currentTime;
    private bool isGameOver = false;

    [SerializeField] private PlayerController playerController;

    // 외부에서 읽기만 가능한 현재 시간 프로퍼티
    public float CurrentTime => currentTime;

    // 게임 상태 프로퍼티 (추가로 필요하면 확장 가능)
    public bool IsGameOver => isGameOver;
    public float TargetDiameter => targetDiameter;

    void Start()
    {
        currentTime = totalTime;
        isGameOver = false;
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isGameOver = true;

            float playerDiameter = playerController.GetRadius() * 2f;

            if (playerDiameter >= targetDiameter)
            {
                Debug.Log("게임 클리어!");
                // 클리어 처리 함수 호출(추가 구현 가능)
            }
            else
            {
                Debug.Log("게임 오버!");
                // 게임 오버 처리 함수 호출(추가 구현 가능)
            }
        }
    }
}
