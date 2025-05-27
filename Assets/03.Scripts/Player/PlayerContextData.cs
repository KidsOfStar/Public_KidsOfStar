using UnityEngine;

[System.Serializable]
public class PlayerContextData
{
    public Player PlayerSc { get; private set; }    // 플레이어 스크립트
    public PlayerController Controller { get; private set; }    // 플레이어 컨트롤러 스크립트
    public PlayerFormController FormController { get; private set; }    // 플레이어 형태 변화 스크립트
    public PlayerStateMachine StateMachine { get; private set; }    // 상태 머신 스크립트
    public SpriteRenderer Renderer { get; private set; }
    public Rigidbody2D Rigid { get; private set; }
    public CapsuleCollider2D CapsuleCollider { get; private set; }
    
    // 벽 타기가 가능한 상태인지 여부
    public bool CanCling { get; set; }
    // 마지막 벽 타기 이후의 시간 체크
    public float CanClingTimer = 0;

    // 사다리 타고 내려오면서 충돌 무시 상태가 된 콜라이더
    public Collider2D IgnoredPlatform = null;

    // 생성자
    // 각 상태에서 사용될 데이터를 보관
    public PlayerContextData(Player player, PlayerController con, PlayerFormController formCon,
        PlayerStateMachine machine, SpriteRenderer sr, Rigidbody2D rb, CapsuleCollider2D box)
    {
        this.PlayerSc = player;
        this.Controller = con;
        this.FormController = formCon;
        this.StateMachine = machine;
        this.Renderer = sr;
        this.Rigid = rb;
        this.CapsuleCollider = box;
    }
}