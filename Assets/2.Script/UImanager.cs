using TMPro;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    [Header("플레이어 컨트롤러")]
    [SerializeField] private PlayerController playerController;

    [Header("게임 매니저")]
    [SerializeField] private GameManager gameManager;

    [Header("크기 표시용 텍스트")]
    [SerializeField] private TMP_Text sizeText;

    [Header("타이머 표시용 텍스트")]
    [SerializeField] private TMP_Text timerText;

    [Header("게임 결과 패널")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;

    private bool resultShown = false;

    private void Update()
    {
        if (playerController == null || gameManager == null)
            return;

        // 크기 UI
        float radius = playerController.GetRadius();
        float diameter = radius * 2f;
        if (sizeText != null)
            sizeText.text = $"Size:\n{diameter:F2}cm";

        // 타이머 UI
        if (timerText != null)
        {
            float currentTime = gameManager.CurrentTime;
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        // 게임 종료 시 결과 UI 표시
        if (gameManager.IsGameOver && !resultShown)
        {
            ShowGameResult();
            resultShown = true;
        }
    }

    private void ShowGameResult()
    {
        float diameter = playerController.GetRadius() * 2f;
        float target = gameManager.TargetDiameter;

        if (diameter >= target)
        {
            if (clearPanel != null)
                clearPanel.SetActive(true);
        }
        else
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }
}
