using UnityEngine;

public class PlayerStateBase : IPlayerState
{
    // 상태 클래스 동작에 필요한 데이터 모음집
    protected PlayerContextData context;
    // 상태 클래스 모음집
    protected PlayerStateFactory factory;


    // 생성자를 통해 데이터를 전달 받고 적용
    public PlayerStateBase(PlayerContextData data, PlayerStateFactory factory)
    {
        this.context = data;
        this.factory = factory;
    }

    /// <summary>
    /// 상태의 시작점
    /// </summary>
    public virtual void OnEnter()
    {
        
    }

    /// <summary>
    /// 다른 상태로 전환되기 직전에 처리할 동작
    /// </summary>
    public virtual void OnExit()
    {
        
    }

    /// <summary>
    /// 이 상태에서 Update 수행되어야 할 동작
    /// </summary>
    public virtual void OnUpdate()
    {
        // 마지막 벽 타기 이후 얼마나 지났는지를 체크 하기 위함
        context.CanClingTimer += Time.deltaTime;
        // 플레이어 조작 잠금 상태면 return
        if (!context.Controller.IsControllable) return;

        // 플레이어가 공중에 있고
        if(!context.Controller.IsGround)
        {
            // 고양이 형태 || 이동 키 입력중 || 벽타기 가능한 상태
            if (context.FormController.CurFormData.playerFormType == PlayerFormType.Cat && 
                context.Controller.MoveDir.x != 0 && 
                context.CanCling &&
                context.Rigid.velocity.y < 
                context.Controller.WallJumpForce - context.Controller.WallJumpCut)
            {
                WallTouchCheck();
            }
        }
    }

    /// <summary>
    /// 이 상태에서 수행되어야 하는 물리 동작
    /// </summary>
    public virtual void OnFixedUpdate()
    {
        if (!context.Controller.IsControllable) return;
    }

    /// <summary>
    /// 벽 타기가 가능한 벽이 유효 거리 안에 있는지 체크
    /// </summary>
    protected void WallTouchCheck()
    {
        // 현재 이동 방향
        Vector2 dir = new Vector2(Mathf.Sign(context.Controller.MoveDir.x), 0);

        // 레이를 발사할 콜라이더 가장자리 위치 값 구하기
        Vector2 origin = context.CapsuleCollider.bounds.center;
        origin.x += dir.x * (context.CapsuleCollider.bounds.extents.x + 0.012f);

        // 레이 길이
        float rayLength = 0.1f;

        // 레이캐스트
        RaycastHit2D checkHit = Physics2D.Raycast(origin, dir,
            rayLength, context.Controller.GroundLayer);
        //Debug.DrawRay(origin, dir * rayLength, Color.red, 1f);

        // 벽이 감지 됐다면
        if (checkHit.collider != null)
        {
            // 마지막 벽타기 이후 미리 설정해둔 시간이 지났다면
            if (context.CanClingTimer >= context.Controller.CatClingTimer)
            {
                // 초기화
                context.CanClingTimer = 0f;
                // 벽 타기 상태로 전환
                context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.WallCling));
            }
        }
    }
}
