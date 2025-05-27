using System.Collections;
using UnityEngine;
public interface IWeightable
{
    float GetWeight();
}

public class Box : MonoBehaviour, IWeightable, ILeafJumpable
{
    private float baseWight = 2f;
    public float boxWeight;

    [Tooltip("Player 레이어와의 충돌을 무시할 시간")]
    public float ignoreDuration = 0.5f;

    [SerializeField] private Collider2D coll;
    [SerializeField] private bool canOnWeightable;

    private Rigidbody2D rb;
    private int boxLayer;
    private int playerLayer;
    public Vector3 boxBasePos;

    void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 1f;
        boxWeight = baseWight;
        boxLayer = gameObject.layer;
        playerLayer = LayerMask.NameToLayer("Player");
    }

    // 박스의 Weight를 가져오는 메서드로 IWeightable로 구현
    public float GetWeight()
    {
        return boxWeight;
    }
    
    public void StartLeafJump(Vector2 dropPosition, float jumpPower)
    {
        // 레이어간 충돌을 무시하는 코루틴 시작
        StartCoroutine(TemporaryIgnorePlayer(ignoreDuration));

        // 물리 초기화
        rb.gravityScale = 1f;
        
        Vector2 impulse = dropPosition * jumpPower;

        rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    // 특정 레이어 간의 충돌을 무시했다가 돌리는 코루틴
    private IEnumerator TemporaryIgnorePlayer(float duration)
    {
        // 박스, 플레이어 레이어간 충돌 판정을 무시
        Physics2D.IgnoreLayerCollision(boxLayer, playerLayer, true);

        // duration만큼만 무시
        yield return new WaitForSeconds(duration);

        // 박스, 플레이어 레이어간 충돌 판정 다시 허용
        Physics2D.IgnoreLayerCollision(boxLayer, playerLayer, false);
    }

    public void ResetPosition()
    {
        this.gameObject.transform.position = boxBasePos;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!canOnWeightable) return;
        if (!other.gameObject.TryGetComponent(out IWeightable weightable)) return;

        if (IsOnBox(other.collider))
        {
            var weightablePos = other.transform.position;
            var fixedPositionY = coll.bounds.max.y - 0.1f;
            var fixedPosition = new Vector2(weightablePos.x, fixedPositionY);
            other.transform.position = fixedPosition;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!canOnWeightable) return;
        if (!other.gameObject.TryGetComponent(out IWeightable weightable)) return;

        if (IsOnBox(other.collider))
        {
            // 플레이어의 부모가 박스가 아니라면 박스를 부모로 설정
            if (other.transform.parent == transform)
                return;

            other.transform.SetParent(transform);
            boxWeight = baseWight + weightable.GetWeight();
        }
        else
        {
            if (other.transform.parent != transform)
                return;

            other.transform.SetParent(null);
            boxWeight = baseWight;
        }
    }

    private bool IsOnBox(Collider2D weightable)
    {
        var obj = weightable.bounds;
        var box = coll.bounds;
        
        bool overlapY = !(Mathf.Abs(obj.min.y - box.max.y) > 0.1f);
        
        float overlapX = Mathf.Min(obj.max.x, box.max.x) 
                         - Mathf.Max(obj.min.x, box.min.x);
        bool halfWidthOverlap = overlapX >= (obj.size.x * 0.5f);

        return overlapY && halfWidthOverlap;
    }
}
