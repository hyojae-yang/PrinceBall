using UnityEngine;

/// <summary>
/// ���� ���� �ð�, ��ǥ ũ�� ����, ��� ó�� (Ŭ���� or ����)
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("��ǥ ũ�� (���� ����)")]
    public float targetDiameter = 10f;

    [Header("���� �ð� (��)")]
    public float totalTime = 60f;

    [SerializeField] private PlayerController playerController;

    // ���� �ð� �� ����
    private float currentTime;
    private bool isGameOver = false;
    private bool isCleared = false;

    // UIManager �� �ܺ� ���ٿ�
    public float CurrentTime => currentTime;
    public bool IsGameOver => isGameOver;
    public bool IsCleared => isCleared;
    public float TargetDiameter => targetDiameter;

    // ��� Ÿ�� enum
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

        // ? �ǽð� ũ�� üũ
        float playerDiameter = playerController.GetRadius() * 2f;
        if (playerDiameter >= targetDiameter)
        {
            RaiseGameClear();  // ���� �޼� �� ��� Ŭ����
            return;
        }

        // ���ѽð� ����
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            RaiseGameOver();  // �ð� �ʰ� �� ����
        }
    }

    // ���� ���� ó�� �Լ�
    private void RaiseGameOver()
    {
        isGameOver = true;

        float playerDiameter = playerController.GetRadius() * 2f;

        if (playerDiameter >= targetDiameter)
        {
            isCleared = true;
            result = GameResult.Cleared;
            Debug.Log("���� Ŭ����!");
        }
        else
        {
            isCleared = false;
            result = GameResult.Failed;
            Debug.Log("���� ����!");
        }
    }
    private void RaiseGameClear()
    {
        isGameOver = true;
        isCleared = true;
        result = GameResult.Cleared;

        Debug.Log("��ǥ ũ�� �޼� �� ���� Ŭ����!");
    }

}
