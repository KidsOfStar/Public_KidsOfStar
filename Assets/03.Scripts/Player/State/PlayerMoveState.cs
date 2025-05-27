using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(PlayerContextData data, PlayerStateFactory factory) : base(data, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        // 애니메이터의 Move 파라미터를 true로
        context.Controller.Anim.SetBool(PlayerAnimHash.AnimMove, true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // 땅에 닿은 상태 && 이동 키 입력 없음
        if(context.Controller.IsGround && Mathf.Abs(context.Controller.MoveDir.x) < 0.1f)
        {
            // 대기 상태로 전환
            context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.Idle));
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // 이동 동작
        context.Controller.Move();
    }

    public override void OnExit()
    {
        base.OnExit();
        context.Controller.Move();
    }
}