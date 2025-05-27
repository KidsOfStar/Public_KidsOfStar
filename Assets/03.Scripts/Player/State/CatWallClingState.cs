using UnityEngine;

public class CatWallClingState : PlayerStateBase
{
    public CatWallClingState(PlayerContextData data, PlayerStateFactory factory) : base(data, factory)
    {
    }

    public override void OnEnter()
    {
        // 플레이어 일시적으로 정지
        context.Rigid.velocity = Vector2.zero;
        WallRatationSet();
        // 곧바로 다시 벽을 타지 못하게 잠금
        context.CanCling = false;
    }

    /// <summary>
    /// 벽을 타는 방향에 따라서 스프라이트 회전
    /// </summary>
    private void WallRatationSet()
    {
        // 현재 입력 방향
        float dirx = Mathf.Sign(context.Controller.MoveDir.x);
        Vector3 rot = Vector3.zero;

        // 오른쪽 방향일 때
        if (dirx > 0)
        {
            rot = new Vector3(0, 0, 90);
        }
        else if (dirx < 0)
        {
            // 왼쪽 방향일 때
            rot = new Vector3(0, 0, -90);
        }

        context.Controller.transform.GetChild(0).rotation = Quaternion.Euler(rot);
        context.Controller.SetCollider(CapsuleDirection2D.Vertical);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 땅에 닿는다면
        if(context.Controller.IsGround)
        {
            // 대기 상태로 전환
            context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.Idle));
            return;
        }

        // 이동 키 입력 없음 || 이동 키 입력 방향에 벽 감지 안 됨
        if(context.Controller.MoveDir.x == 0 || !IsWallTouchCheck())
        {
            // 대기 상태로 전환
            context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.Idle));
            return;
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (context.StateMachine.CurState != this) return;
        // 이동 동작 실행
        context.Controller.Move();
        context.Rigid.velocity = new Vector2(context.Rigid.velocity.x, 0);
    }

    /// <summary>
    /// 이동 방향에 벽이 있는지 체크
    /// </summary>
    /// <returns>벽 여부</returns>
    bool IsWallTouchCheck()
    {
        Vector2 dir = new Vector2(Mathf.Sign(context.Controller.MoveDir.x), 0);
        Vector2 origin = context.CapsuleCollider.bounds.center;
        origin.y = context.CapsuleCollider.bounds.min.y + 0.05f;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir,
            context.CapsuleCollider.bounds.size.x * 1.5f, context.Controller.GroundLayer);
        //Debug.DrawRay(origin, dir * context.BoxCollider.bounds.size.x * 1.5f,
        //    Color.green, 1f);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnExit()
    {
        // 각도를 원래대로
        context.Controller.transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);
        context.Controller.SetCollider(CapsuleDirection2D.Horizontal);
    }
}