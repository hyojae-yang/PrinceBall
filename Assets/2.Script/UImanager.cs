using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// ���� UI ǥ�� �� ��� �г� ����
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;

    [Header("UI �ؽ�Ʈ")]
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text targetText;

    [Header("��� �г�")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject gameOverPanel;

    private bool resultShown = false;

    void Start()
    {
        // ��ǥ ũ�� �ؽ�Ʈ ����
        if (gameManager != null && targetText != null)
        {
            float target = gameManager.TargetDiameter;
            targetText.text = $"Target: {FormatSize(target)}";
        }

        // ��� �г� ��Ȱ��ȭ
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
        Debug.Log($"[UI] ���� ��� ó�� - Cleared: {gameManager.IsCleared}");

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
    /// �Ǽ� ���� ���͸� �޾� M, CM, MM ���� ���ڿ��� ��ȯ�մϴ�.
    /// ��, 1m �̻��̸� mm�� �����ǰ�, m�� 0�̸� m ������ ǥ������ �ʽ��ϴ�.
    /// </summary>
    private string FormatSize(float meters)
    {
        int m = Mathf.FloorToInt(meters);                             // ���� ����
        int cm = Mathf.FloorToInt((meters * 100f) % 100f);            // ��Ƽ����
        int mm = Mathf.FloorToInt((meters * 1000f) % 10f);       // �и����� (10�� �ڸ��� ���)

        List<string> parts = new List<string>();

        if (m > 0)
            parts.Add($"{m}m");
        if (cm > 0 || m == 0) // m�� 0�� �� cm�� ������ ��
            parts.Add($"{cm}cm");
        if (m < 1 && mm > 0)  // 1m �̸��� ���� mm ǥ��
            parts.Add($"{mm}mm");

        return string.Join(" ", parts);
    }

}
