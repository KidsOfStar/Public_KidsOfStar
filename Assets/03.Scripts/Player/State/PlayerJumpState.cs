using UnityEngine;

public class PlayerJumpState : PlayerJumpBaseState
{
    public PlayerJumpState(PlayerContextData data, PlayerStateFactory factory) : base(data, factory) { }

    public override void OnEnter()
    {
        //context.Controller.Anim.SetTrigger(PlayerAnimHash.AnimJump);
        // 리지드바디로 실제 점프 동작 실행
        context.Rigid.AddForce(Vector2.up * context.Controller.JumpForce, ForceMode2D.Impulse);
    }
}