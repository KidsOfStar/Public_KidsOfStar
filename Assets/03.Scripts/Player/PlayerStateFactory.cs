using System.Collections.Generic;

// 각 상태를 구분하기 위한 타입 
public enum PlayerStateType
{
    Idle,
    Move,
    Jump,
    WallCling,
    WallJump,
    OnLadder
}

public class PlayerStateFactory
{
    // 상태 딕셔너리
    private Dictionary<PlayerStateType, IPlayerState> stateDictionary;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="context">상태에서 공통적으로 쓰일 데이터를 모아둔 context</param>
    public PlayerStateFactory(PlayerContextData context)
    {
        // 각 상태 클래스의 인스턴스를 이 시점에 생성
        stateDictionary = new Dictionary<PlayerStateType, IPlayerState>()
        {
            { PlayerStateType.Idle, new PlayerIdleState(context, this) },
            { PlayerStateType.Move, new PlayerMoveState(context, this) },
            { PlayerStateType.Jump, new PlayerJumpState(context, this) },
            { PlayerStateType.WallCling, new CatWallClingState(context, this) },
            { PlayerStateType.WallJump, new CatWallJumpState(context, this) },
            { PlayerStateType.OnLadder, new OnLadderState(context, this) },
        };
    }

    /// <summary>
    /// 상태 전환 함수
    /// </summary>
    /// <param name="type">전환하려는 상태의 타입</param>
    /// <returns>전환하려는 상태의 인스터스</returns>
    public IPlayerState GetPlayerState(PlayerStateType type)
    {
        return stateDictionary[type];
    }
}