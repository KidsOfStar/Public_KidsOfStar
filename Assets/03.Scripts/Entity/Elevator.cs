using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class Elevator : MonoBehaviour
{
    // 이동속도 및 방향을 설정할 수 있어야 함
    [Header("Components")]
    [SerializeField] private Collider2D coll;

    [SerializeField] private SpriteRenderer sprite;

    [Header("Elevator Settings")]
    [SerializeField] private bool isLocked = true;

    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private Direction direction = Direction.None;

    private readonly WaitForFixedUpdate waitForFixedUpdate = new();
    private readonly WaitForSeconds repairTime = new(5f);

    private readonly List<IWeightable> weightables = new();
    private const float MoveWaitTime = 2f;
    private const float MaxWeight = 3f;
    private const float VerticalMargin = 0.1f;
    private const string BreakWarning = "너무 무거워서 떨어질 것 같다.";

    private Vector3 prevPos;
    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;
        targetPos = GetTargetPosition();

        if (!isLocked)
            StartCoroutine(Move(false));
    }

    // 배선판 퍼즐 클리어 시 사용 할 함수
    public void UnlockElevator()
    {
        isLocked = false;
        StartCoroutine(Move());
    }

    private IEnumerator Move(bool isPlaySound = true)
    {
        yield return MoveWaitRoutine();

        if (isPlaySound)
            Managers.Instance.SoundManager.PlaySfx(SfxSoundType.ElevatorMove);
        
        while (true)
        {
            yield return MoveRoutine(startPos, targetPos);
            yield return MoveWaitRoutine();

            yield return MoveRoutine(targetPos, startPos);
            yield return MoveWaitRoutine();
        }
    }

    private IEnumerator MoveWaitRoutine()
    {
        float elapsed = 0f;
        while (elapsed < MoveWaitTime)
        {
            if (GetCurrentWeight() > MaxWeight)
                yield return StartCoroutine(BreakSequence());
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveRoutine(Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float duration = distance / speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 과부하 감지: true면 BreakSequence 실행 후 리턴하지 않고 그대로 이 지점에서 재개
            if (GetCurrentWeight() > MaxWeight)
                yield return StartCoroutine(BreakSequence());

            // 진행도에 맞춰 위치 보간
            float t = elapsed / duration;
            Vector3 nextPos = Vector3.Lerp(from, to, t);

            // rigid.MovePosition(nextPos);
            transform.position = nextPos;

            elapsed += Time.deltaTime;
            yield return waitForFixedUpdate;
        }

        // 정확히 to 위치 보정
        transform.position = to;
    }

    private IEnumerator BreakSequence()
    {
        const float blinkInterval = 0.5f; // 0.5초씩 색상 전환
        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
                Managers.Instance.DialogueManager.SetInteractObjectDialog(BreakWarning);
            
            // 1) White → Red
            float t = 0f;
            while (t < blinkInterval)
            {
                if (GetCurrentWeight() < MaxWeight)
                {
                    // 과부하 해제 시 즉시 종료
                    sprite.color = Color.white;
                    yield break;
                }

                sprite.color = Color.Lerp(Color.white, Color.red, t / blinkInterval);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            sprite.color = Color.red;

            // 2) Red → White
            t = 0f;
            while (t < blinkInterval)
            {
                if (GetCurrentWeight() < MaxWeight)
                {
                    sprite.color = Color.white;
                    yield break;
                }

                sprite.color = Color.Lerp(Color.red, Color.white, t / blinkInterval);
                t += Time.deltaTime;
                yield return null;
            }
            sprite.color = Color.white;
        }

        // 3초 경고 후에도 과부하 상태라면 완전 고장
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.BrokenElevator);
        
        // 복구 대기
        yield return repairTime;

        // 고장 해제
        sprite.color = Color.white;
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetPosition = startPos;

        switch (direction)
        {
            case Direction.Up:
                targetPosition.y += distance;
                break;
            case Direction.Down:
                targetPosition.y -= distance;
                break;
            case Direction.Left:
                targetPosition.x -= distance;
                break;
            case Direction.Right:
                targetPosition.x += distance;
                break;
            case Direction.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return targetPosition;
    }

    private float GetCurrentWeight()
    {
        float totalWeight = 0f;

        for (int i = 0; i < weightables.Count; i++)
        {
            var weightable = weightables[i];
            totalWeight += weightable.GetWeight();
        }

        return totalWeight;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent(out IWeightable weightable)) return;

        if (IsOnElevator(other.collider))
        {
            var weightableTr = other.transform;
            var weightablePos = weightableTr.position;
            var fixedPositionY = coll.bounds.max.y - 0.1f;
            var fixedPosition = new Vector2(weightablePos.x, fixedPositionY);
            other.transform.position = fixedPosition;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent(out IWeightable weightable)) return;

        // 물체가 엘레베이터에 올라탄 상태라면
        if (IsOnElevator(other.collider))
        {
            if (weightables.Contains(weightable)) return;
            other.transform.SetParent(transform);
            weightables.Add(weightable);
        }
        // 물체가 엘레베이터에 올라탄 상태가 아니라면
        else
        {
            other.transform.SetParent(null);
            if (weightables.Contains(weightable))
                weightables.Remove(weightable);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent(out IWeightable weightable)) return;

        other.transform.SetParent(null);
        if (weightables.Contains(weightable))
            weightables.Remove(weightable);
    }

    private bool IsOnElevator(Collider2D weightable)
    {
        var obj = weightable.bounds;
        var elevator = coll.bounds;

        // 물체의 바닥면이 엘레베이터의 바닥면보다 맞지 않으면 false
        bool overlapY = !(Mathf.Abs(obj.min.y - elevator.max.y) > VerticalMargin);
        
        float overlapX = Mathf.Min(obj.max.x, elevator.max.x) 
                         - Mathf.Max(obj.min.x, elevator.min.x);
        bool halfWidthOverlap = overlapX >= (obj.size.x * 0.7f);

        return overlapY && halfWidthOverlap;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}