using UnityEngine;

public class Obstacle : ScrollObject
{
    // 중간 수초의 y축 Offset 크기
    private float mediumYOffset = 0.1f;

    // 큰 수초의 y축 Offset 크기
    private float largeYOffset = 0.2f;

    // 장애물의 y축 스폰되는 고정 위치
    private float fixedPosY = -2.75f;

    public void InitObstacle(Vector3 spawnPosition, ObstacleType chosenType)
    {
        if(chosenType == ObstacleType.Stone)
        {
            // 돌멩이일때 장애물 y축 스폰되는 고정 위치
            fixedPosY = -3.05f;
        }
        if (chosenType == ObstacleType.MediumSeaweed)
        {
            fixedPosY += mediumYOffset;
        }
        else if (chosenType == ObstacleType.LargeSeaweed)
        {
            fixedPosY += largeYOffset;
        }

        Vector3 pos = new Vector3(spawnPosition.x, fixedPosY, 0f);
        transform.position = pos;
        // SetupAnimation();  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Managers.Instance.GameManager.GameOver();
        }
    }
}
