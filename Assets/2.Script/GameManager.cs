using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("��ǥ ũ�� (���� ����)")]
    public float targetDiameter = 10f;

    [Header("���� �ð� (��)")]
    public float totalTime = 60f;

    private float currentTime;
    private bool isGameOver = false;

    [SerializeField] private PlayerController playerController;

    // �ܺο��� �б⸸ ������ ���� �ð� ������Ƽ
    public float CurrentTime => currentTime;

    // ���� ���� ������Ƽ (�߰��� �ʿ��ϸ� Ȯ�� ����)
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
                Debug.Log("���� Ŭ����!");
                // Ŭ���� ó�� �Լ� ȣ��(�߰� ���� ����)
            }
            else
            {
                Debug.Log("���� ����!");
                // ���� ���� ó�� �Լ� ȣ��(�߰� ���� ����)
            }
        }
    }
}
