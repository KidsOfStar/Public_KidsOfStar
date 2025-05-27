using UnityEngine;

public class ObstacleDespawner : MonoBehaviour
{
    public float tilemapWidth;
    public ObstaclesSpawner obstaclesSpawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Managers.Instance.PoolManager.Despawn(collision.gameObject);

            if (obstaclesSpawner != null)
            {
                obstaclesSpawner.OnObstacleDespawned();
            }
        }

        if (collision.CompareTag("Loopable"))
        {
            var tilemap = collision.transform.parent;
            float newX = tilemap.position.x + tilemapWidth * 2f; // 현재 기준에서 오른쪽으로 정확히 한 사이클 이동
            tilemap.position = new Vector3(newX, tilemap.position.y, tilemap.position.z);
        }
    }
}