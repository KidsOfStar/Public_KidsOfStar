using System.Collections;
using UnityEngine;

public interface ILeafJumpable
{
    void StartLeafJump(Vector2 targetPosition, float duration);
}
public class Leaf : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("목표 이동 지점")] public Vector3 dropPosition;
    [Tooltip("Jump의 Power")] public float jumpPower = 5;

    [Header("Respawn Settings")]
    [Tooltip("Leaf가 떨어지고 다시 스폰되기 전까지 대기할 시간 (초)")]
    [SerializeField] private float respawnDelay = 5f;
    [Tooltip("Leaf가 떨어지고 사라지기까지의 시간 (초)")]
    [SerializeField] private float fallDuration = 0.3f;

    [Header("충돌 감지 레이어")]
    public LayerMask obstacleMask;

    private bool isUsed = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private Vector3 originalPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalPosition = transform.position;
        rb.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (isUsed) return;
        if ((collision.collider.CompareTag("Player") || collision.collider.CompareTag("Box")) && CheckBoundary(collision.transform))
        {
            if (collision.collider.TryGetComponent<ILeafJumpable>(out var jumpable))
            {
                Managers.Instance.SoundManager.PlaySfx(SfxSoundType.LeafTrampoline);
                jumpable.StartLeafJump(dropPosition,jumpPower);
                isUsed = true;
                StartCoroutine(DropAndRespawn());
            }
        }
    }

    // 전체면적이 닿았는지 경계 검사(0.3f만큼의 수직 허용 오차)
    private bool CheckBoundary(Transform target)
    {
        float verticalTolerance = 0.3f;
        return target.position.y >= transform.position.y - verticalTolerance;
    }

    private IEnumerator DropAndRespawn()
    {
        // 중력 활성화하여 떨어지게 함
        rb.gravityScale = 1f;

        // 사라지기 전까지 대기
        yield return new WaitForSeconds(fallDuration);

        // 비활성화 대신 렌더러와 콜라이더만 끄기
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // 원위치로 리셋
        ResetLeaf();
    }

    public void ResetLeaf() 
    {
        isUsed = false;
        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
        sr.enabled = true;
        col.enabled = true;
        rb.gravityScale = 0f;
    }
}
