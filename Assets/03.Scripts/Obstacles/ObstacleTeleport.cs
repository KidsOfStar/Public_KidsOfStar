using System.Collections;
using UnityEngine;

public class ObstacleTeleport : MonoBehaviour
{
    [Header("Teleport pos")]
    public Vector3 transTeleportTarget; // 텔레포트할 위치
    public GameObject teleportTarget; // 텔레포트 이펙트

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 오브젝트가 해당 위치일 때
        StartCoroutine(Teleport()); // 텔레포트 이펙트 시작
    }

    private IEnumerator Teleport()
    {
        yield return new WaitForSeconds(5f); // 5초 대기
        teleportTarget.transform.position = transTeleportTarget; // 텔레포트 이펙트 위치 설정
    }
}
