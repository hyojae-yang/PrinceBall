using UnityEngine;

public class Rat : Enemy
{
    [Header("자동 이동 설정")]
    public Vector3 moveDirection = Vector3.forward;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private int moveDirSign = 1;

    protected void Awake()
    {
        base.Awake();
        startPos = transform.position;
    }
    void Update()
    {
        if (isAttached)
            return;
        MoveEnemy();
    }
    private void MoveEnemy()
    {
        Vector3 dirNormalized = moveDirection.normalized;
        Vector3 offset = dirNormalized * moveSpeed * moveDirSign * Time.deltaTime;
        transform.position += offset;

        Vector3 toCurrent = transform.position - startPos;
        float projectedDistance = Vector3.Dot(toCurrent, dirNormalized);

        if (projectedDistance > moveDistance)
        {
            moveDirSign = -1;
            transform.position = startPos + dirNormalized * moveDistance;
            transform.rotation = Quaternion.Euler(0f, 0f, 270f);
        }
        else if (projectedDistance < 0f)
        {
            moveDirSign = 1;
            transform.position = startPos;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }
}
