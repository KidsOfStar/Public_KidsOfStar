using UnityEngine;

public static class PlayerAnimHash
{
    // 애니메이터 Ground 파라미터
    public static readonly int AnimGround = Animator.StringToHash("Ground");
    // 애니메이터 Move 파라미터 
    public static readonly int AnimMove = Animator.StringToHash("Move");
    public static readonly int AnimJump = Animator.StringToHash("Jump");
}