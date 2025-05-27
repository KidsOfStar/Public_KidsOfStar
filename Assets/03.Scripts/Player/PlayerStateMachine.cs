using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private Player player;
    public Player Player { set {  player = value; } }
    // 현재 상태
    private IPlayerState curState;
    public IPlayerState CurState { get { return curState; } }
    [SerializeField, Tooltip("상태 클래스에서 접근해야 하는 데이터 모음")] private PlayerContextData contextData;
    public PlayerContextData ContextData { get { return contextData; } }
    // 상태 클래스 관리 스크립트
    private PlayerStateFactory factory;
    public PlayerStateFactory Factory {  get { return factory; } }

    // 초기화
    public void Init(Player player)
    {
        this.player = player;
        // 데이터 모음 생성
        contextData = new PlayerContextData(player, player.Controller, player.FormControl, this,
            GetComponentInChildren<SpriteRenderer>(), GetComponent<Rigidbody2D>(), GetComponent<CapsuleCollider2D>());
        // 상태 클래스 생성
        factory = new PlayerStateFactory(contextData);
        // 대기 상태로 시작
        ChangeState(factory.GetPlayerState(PlayerStateType.Idle));
    }

    void Update()
    {
        // 플레이어 조작 잠금 상태이면 return
        if (!player.Controller.IsControllable) return;

         // 현재 상태의 OnUpdate 동작
        curState?.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (!player.Controller.IsControllable) return;

        // 현재 상태의 OnFixedUpdate 동작
        curState?.OnFixedUpdate();
    }

    /// <summary>
    /// 상태 전환 함수
    /// </summary>
    /// <param name="nextState">전환할 상태</param>
    public void ChangeState(IPlayerState nextState)
    {
        // 전환하려는 상태가 현재 상태와 같으면 return
        if (curState == nextState) return;

        // 현재 상태의 OnExit 동작
        curState?.OnExit();
        // 현재 상태 변경
        curState = nextState;
        // 새로운 현재 상태의 OnEnter 동작
        curState?.OnEnter();
        //EditorLog.Log(curState.ToString());
    }
}
