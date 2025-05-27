using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerContextData data, PlayerStateFactory factory) : base(data, factory) { }

    public override void OnEnter()
    {
        base.OnEnter();

        // 추격전 모드가 아니라면
        if (!context.Controller.IsChaseMode)
        {
            // 애니메이터의 Move 파라미터를 false로
            context.Controller.Anim.SetBool(PlayerAnimHash.AnimMove, false);
        }

        if(context.Controller.IsGround)
        {
            context.Rigid.velocity = Vector2.zero;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 이동 키 입력 중 && 땅에 닿은 상태
        if(Mathf.Abs(context.Controller.MoveDir.x) > 0.1f && context.Controller.IsGround)
        {
            // 이동 상태로 전환
            context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.Move));
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // 공중에 있는 상태라면
        if(!context.Controller.IsGround)
        {
            // 이동
            context.Controller.Move();
        }
        else
        {
            //// 땅 위에 서 있는 상태라면
            //// 정지
            //context.Rigid.velocity = Vector2.zero;
        }
    }
}