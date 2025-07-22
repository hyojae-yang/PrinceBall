using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// 게임 UI 표시 및 결과 패널 관리
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;

    [Header("UI 텍스트")]
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text targetText;

    [Header("결과 패널")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject gameOverPanel;

    private bool resultShown = false;

    void Start()
    {
        // 목표 크기 텍스트 설정
        if (gameManager != null && targetText != null)
        {
            float target = gameManager.TargetDiameter;
            targetText.text = $"Target: {FormatSize(target)}";
        }

        // 결과 패널 비활성화
        clearPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
    }

    void Update()
    {
        if (playerController != null && sizeText != null)
        {
            float radius = playerController.GetRadius();
            float diameter = radius * 2f;
            sizeText.text = $"Size:\n{FormatSize(diameter)}";
        }

        if (gameManager != null && timeText != null)
        {
            float time = Mathf.Max(0f, gameManager.CurrentTime);
            int min = Mathf.FloorToInt(time / 60f);
            int sec = Mathf.FloorToInt(time % 60f);
            timeText.text = $"Time: {min:00}:{sec:00}";
        }

        if (!resultShown && gameManager != null && gameManager.IsGameOver)
        {
            ShowGameResult();
            resultShown = true;
        }
    }

    private void ShowGameResult()
    {
        Debug.Log($"[UI] 게임 결과 처리 - Cleared: {gameManager.IsCleared}");

        if (gameManager.IsCleared)
        {
            clearPanel?.SetActive(true);
        }
        else
        {
            gameOverPanel?.SetActive(true);
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// 실수 단위 미터를 받아 M, CM, MM 형식 문자열로 변환합니다.
    /// 단, 1m 이상이면 mm는 생략되고, m이 0이면 m 단위는 표시하지 않습니다.
    /// </summary>
    private string FormatSize(float meters)
    {
        int m = Mathf.FloorToInt(meters);                             // 정수 미터
        int cm = Mathf.FloorToInt((meters * 100f) % 100f);            // 센티미터
        int mm = Mathf.FloorToInt((meters * 1000f) % 10f);       // 밀리미터 (10의 자리로 계산)

        List<string> parts = new List<string>();

        if (m > 0)
            parts.Add($"{m}m");
        if (cm > 0 || m == 0) // m이 0일 땐 cm라도 보여야 함
            parts.Add($"{cm}cm");
        if (m < 1 && mm > 0)  // 1m 미만일 때만 mm 표시
            parts.Add($"{mm}mm");

        return string.Join(" ", parts);
    }

}
