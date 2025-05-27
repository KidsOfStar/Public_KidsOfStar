using UnityEngine;

public class PlayerGroundState : PlayerStateBase
{
    public PlayerGroundState(PlayerContextData data, PlayerStateFactory factory) : base(data, factory)
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 플레이어가 땅에 닿은 상태 && 플레이어가 사다리에 닿은 상태
        if(context.Controller.TouchLadder && context.Controller.IsGround)
        {
            CheckLadderClimb();
        }
    }

    /// <summary>
    /// 사다리 상태로 전환할 수 있는 조건인지 체크
    /// </summary>
    private void CheckLadderClimb()
    {
        // 상하 이동 키를 입력하지 않는 상태면 return
        if (Mathf.Abs(context.Controller.MoveDir.y) < 0.1f) return;

        Vector2 origin = context.CapsuleCollider.bounds.center;
        origin.y = context.CapsuleCollider.bounds.min.y + 0.03f;
        float rayLength = 0.1f;
        // 닿은 땅 오브젝트의 PlatformEffector2D 여부
        bool onEffector = false;

        // isGround 체크와 같은 조건으로 레이 캐스트
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength,
            context.Controller.GroundLayer);
        //Debug.DrawRay(origin, Vector2.down * rayLength, Color.red, 1f);
        // 하강 시에 딛고 있던 플랫폼의 플랫폼이펙터를 연결할 변수
        PlatformEffector2D effector = null;

        if(hit.collider != null)
        {
            // 체크
            onEffector = hit.collider.TryGetComponent<PlatformEffector2D>(out effector);
        }

        // 플레이어의 현재 형태가 은신 형태
        if(context.FormController.CurFormData.playerFormType == PlayerFormType.Hide)
        {
            // 위쪽 방향으로 입력 중이라면
            if(!onEffector && context.Controller.MoveDir.y > 0)
            {
                // 상태 전환
                context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.OnLadder));
            }
            else if(onEffector && context.Controller.MoveDir.y < 0)
            {
                // 아래 방향으로 입력 중이라면
                // 딛고 있는 플랫폼과의 충돌을 일시적으로 무시
                effector.surfaceArc = 0f;
                // 상태 전환
                context.StateMachine.ChangeState(factory.GetPlayerState(PlayerStateType.OnLadder));
                // 물리 처리 무시된 플랫폼의 콜라이더를 변수에 캐싱
                context.IgnoredPlatform = hit.collider;
            }
        }
        else
        {
            // 플레이어의 현재 형태가 은신 형태가 아닐 경우
            var popup = Managers.Instance.UIManager.Show<WarningPopup>(WarningType.Ladder);
            Vector3 worldAboveHead = context.Controller.transform.position 
                + new Vector3(0, 2, 0);
            popup.SetScreenPosition(worldAboveHead);
        }
    }

    public override void OnEnter()
    {
        // 한 번 땅에 내려가면 다시 벽 타기가 가능하도록
        context.CanCling = true;
    }
}