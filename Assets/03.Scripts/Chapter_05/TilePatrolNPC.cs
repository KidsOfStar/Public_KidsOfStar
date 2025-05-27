using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePatrolNPC : MonoBehaviour
{
    public Tilemap tilemap; // 참조하는 타일맵
    public float moveSpeed = 3f;

    // 실수 기반 월드 좌표로 구성된 경로
    public List<Vector3> patrolPath = new();
    private int currentIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        if (patrolPath.Count > 0)
        {
            transform.position = patrolPath[0]; // 시작 위치 설정
        }
    }

    void Update()
    {
        if (!isMoving && patrolPath.Count > 0)
        {
            StartCoroutine(MoveToPosition(patrolPath[currentIndex]));
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos; // 위치 보정
        currentIndex = (currentIndex + 1) % patrolPath.Count;
        yield return new WaitForSeconds(0.1f);
        isMoving = false;
    }
}
