using UnityEngine;

public class OnLadderState : PlayerStateBase
{
    // 플레이어의 본래 중력 값
    private float gravityScale = 0;

    public OnLadderState(PlayerContextData data, PlayerStateFactory factory) : base(data, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        // 원래의 중력 값을 변수에 캐싱
        gravityScale = context.Rigid.gravityScale;
        // 중력 값을 0으로 전환
        context.Rigid.gravityScale = 0f;
        // 본래 있었을지도 모를 가속도 제거
        context.Rigid.velocity = Vector2.zero;
        // 땅체크 중단
        context.Controller.GroundCheckLocked = true;
        //if(context.IgnoredPlatform != null)
        //{
        //    Physics2D.IgnoreCollision(context.BoxCollider, context.IgnoredPlatform, true);
        //}
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 상하 이동키 입력이 있음
        if(Mathf.Abs(context.Controller.MoveDir.y) > 0.1f)
        {
            // 위 방향 키를 입력중이라면
            if(context.Controller.MoveDir.y > 0)
            { 
                // 플레이어가 땅에 닿은 상태 && 사다리 접촉 X
                if(!context.Controller.TouchLadder)
                {
                    context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.Idle));
                }
            }
            else if(context.Controller.MoveDir.y < 0)
            {
                Vector2 boxOrigin = context.CapsuleCollider.bounds.center;
                RaycastHit2D boxHit =
                    Physics2D.BoxCast(boxOrigin, context.CapsuleCollider.bounds.size, 0,
                    Vector2.down, 0.02f, context.Controller.GroundLayer);
                bool isEffector = false;

                if(boxHit.collider != null)
                {
                    isEffector = boxHit.collider.TryGetComponent<PlatformEffector2D>(out _);
                }

                // 아래 방향 키를 입력중이라면
                // 감지 범위의 땅에 이펙터가 없을 경우
                if(!isEffector)
                {
                    Vector2 origin = context.CapsuleCollider.bounds.center;
                    origin.y = context.CapsuleCollider.bounds.min.y + 0.03f;
                    float rayLength = 0.1f;

                    // isGround 체크와 같은 조건으로 레이 캐스트
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength,
                        context.Controller.GroundLayer);

                    // 하강 중에 플랫폼이펙터가 없는 땅에 닿으면
                    if(hit.collider != null &&
                        !hit.collider.TryGetComponent<PlatformEffector2D>(out _))
                    {
                        // 대기 상태로 전환
                        context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.Idle));
                    }
                }
            }
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // 사다리 이동
        context.Controller.ClimbLadder();
    }

    public override void OnExit()
    {
        base.OnExit();
        // 위로 승천(?)하는 혹시 모를 버그 방지용
        context.Rigid.velocity = Vector2.zero;
        // 본래의 중력 값 적용
        context.Rigid.gravityScale = gravityScale;
        // 땅 체크 잠금 해제
        context.Controller.GroundCheckLocked = false;

        // 하강하면서 충돌 처리를 무시하게 된 플랫폼이 있다면
        if(context.IgnoredPlatform != null)
        {
            // 충돌 처리가 되도록 되돌리기
            context.IgnoredPlatform.GetComponent<PlatformEffector2D>().surfaceArc = 180f;
            // 체크를 위해 변수 비워두기
            context.IgnoredPlatform = null;
        }
    }
}