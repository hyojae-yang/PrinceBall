using UnityEngine;

/// <summary>
/// 게임 제한 시간, 목표 크기 판정, 결과 처리 (클리어 or 오버)
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("목표 크기 (지름 기준)")]
    public float targetDiameter = 10f;

    [Header("제한 시간 (초)")]
    public float totalTime = 60f;

    [SerializeField] private PlayerController playerController;

    // 현재 시간 및 상태
    private float currentTime;
    private bool isGameOver = false;
    private bool isCleared = false;

    // UIManager 등 외부 접근용
    public float CurrentTime => currentTime;
    public bool IsGameOver => isGameOver;
    public bool IsCleared => isCleared;
    public float TargetDiameter => targetDiameter;

    // 결과 타입 enum
    public enum GameResult { None, Cleared, Failed }
    private GameResult result = GameResult.None;
    public GameResult Result => result;

    void Start()
    {
        currentTime = totalTime;
        isGameOver = false;
        isCleared = false;
        result = GameResult.None;
    }

    void Update()
    {
        if (isGameOver) return;

        // ? 실시간 크기 체크
        float playerDiameter = playerController.GetRadius() * 2f;
        if (playerDiameter >= targetDiameter)
        {
            RaiseGameClear();  // 조건 달성 시 즉시 클리어
            return;
        }

        // 제한시간 감소
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            RaiseGameOver();  // 시간 초과 시 판정
        }
    }

    // 게임 종료 처리 함수
    private void RaiseGameOver()
    {
        isGameOver = true;

        float playerDiameter = playerController.GetRadius() * 2f;

        if (playerDiameter >= targetDiameter)
        {
            isCleared = true;
            result = GameResult.Cleared;
            Debug.Log("게임 클리어!");
        }
        else
        {
            isCleared = false;
            result = GameResult.Failed;
            Debug.Log("게임 오버!");
        }
    }
    private void RaiseGameClear()
    {
        isGameOver = true;
        isCleared = true;
        result = GameResult.Cleared;

        Debug.Log("목표 크기 달성 → 게임 클리어!");
    }

}
