using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
            targetText.text = $"Target: {target * 100f:F0}cm";
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
            sizeText.text = $"Size: \n{diameter * 100f:F2}cm";
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
}
