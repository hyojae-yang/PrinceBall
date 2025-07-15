using TMPro;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    [Header("�÷��̾� ��Ʈ�ѷ�")]
    [SerializeField] private PlayerController playerController;

    [Header("���� �Ŵ���")]
    [SerializeField] private GameManager gameManager;

    [Header("ũ�� ǥ�ÿ� �ؽ�Ʈ")]
    [SerializeField] private TMP_Text sizeText;

    [Header("Ÿ�̸� ǥ�ÿ� �ؽ�Ʈ")]
    [SerializeField] private TMP_Text timerText;

    [Header("���� ��� �г�")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;

    private bool resultShown = false;

    private void Update()
    {
        if (playerController == null || gameManager == null)
            return;

        // ũ�� UI
        float radius = playerController.GetRadius();
        float diameter = radius * 2f;
        if (sizeText != null)
            sizeText.text = $"Size:\n{diameter:F2}cm";

        // Ÿ�̸� UI
        if (timerText != null)
        {
            float currentTime = gameManager.CurrentTime;
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        // ���� ���� �� ��� UI ǥ��
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
