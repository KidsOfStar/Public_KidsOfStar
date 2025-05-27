using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour,IWeightable, ILeafJumpable
{
    [Header("공통")]
    [SerializeField, Tooltip("플레이어가 땅으로 인식할(+점프 가능한) 레이어")] private LayerMask groundLayer;
    public LayerMask GroundLayer { get { return groundLayer; } }
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;
    public Animator Anim { get { return anim; } }
    [SerializeField, Tooltip("플레이어 캐릭터 이동 속도")] private float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    [SerializeField, Tooltip("플레이어 캐릭터 점프력")] private float jumpForce;
    public float JumpForce
    {
        get { return jumpForce; }
        set { jumpForce = value; }
    }

    [Space, Header("고양이")]
    [SerializeField, Tooltip("벽 점프력")] private float wallJumpForce = 5f;
    public float WallJumpForce { get { return wallJumpForce; } }
    [SerializeField, Tooltip("벽에 붙었다가 떨어졌을 때 다시 붙을 수 있게 되기까지의 쿨타임")] private float catClingTimer = 1f;
    public float CatClingTimer { get { return catClingTimer; } }
    [SerializeField, Tooltip("점프 후 어느 정도의 감속이 있어야 벽점프가 가능한지")]
    private float wallJumpCut = 1f;
    public float WallJumpCut { get { return wallJumpCut; } }

    private Player player;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D rigid;

    // 땅 체크 할지 여부
    private bool groundCheckLocked = false;
    public bool GroundCheckLocked
    {
        get { return groundCheckLocked; }
        set { groundCheckLocked = value; }
    }

    // 플레이어 이동 방향
    // 현재 입력중인 이동 방향
    private Vector2 moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return moveDir; } }
    // 플레이어 캐릭터가 사다리에 닿았는지 여부
    // 플레이어 캐릭터가 사다리에 닿은 상태일 때 true
    private bool touchLadder = false;
    public bool TouchLadder
    {
        get { return touchLadder; }
        set { touchLadder = value; }
    }
    // 플레이어 캐릭터가 땅에 닿았는지 여부
    // 플레이어 캐릭터가 땅에 닿으면 true
    private bool isGround;
    public bool IsGround { get { return isGround; } }
    // 플레이어 캐릭터 조작 가능 여부
    // 플레이어 캐릭터 조작 가능하면 true
    private bool isControllable = true;
    public bool IsControllable
    {
        get { return isControllable; }
        set { isControllable = value; }
    }
    // 추격전 모드 여부
    // 추격 모드면 ture, 평소에는 false
    private bool isChaseMode = false;
    public bool IsChaseMode
    {
        get { return isChaseMode; }
        set { isChaseMode = value; }
    }

    [Header("Push")]
    [SerializeField, Tooltip("밀 수 있는 박스 레이어")]
    private LayerMask pushableLayer;

    private IWeightable objWeight = null;
    // Ray로 감지한 물체의 무게

    private Rigidbody2D objRigid = null;
    // Ray로 감지한 물체의 rb

    [Header("나뭇잎")]
    [Tooltip("Inspector에서 설정할 나뭇잎 힘")]
    public float leafJumpPower;
    private float basePushPower;

    [Header("Player")]
    [Tooltip("Player의 기본 위치")] public Vector3 playerBasePos;

    /// <summary>
    /// 스크립트 초기화 함수
    /// </summary>
    /// <param name="player">Player 스크립트</param>
    public void Init(Player player)
    {
        this.player = player;
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        Managers.Instance.CutSceneManager.OnCutSceneStart += LockPlayer;
        Managers.Instance.DialogueManager.OnDialogStart += LockPlayer;
        Managers.Instance.CutSceneManager.OnCutSceneEnd += UnlockPlayer;
        Managers.Instance.DialogueManager.OnDialogEnd += UnlockPlayer;
    }

    private void Update()
    {
        GroundCheck();
    }

    /// <summary>
    /// 현재 땅(혹은 점프 가능한 오브젝트)에 닿은 상태인지 체크
    /// </summary>
    void GroundCheck()
    {
        // 땅 체크 잠금 여부 체크
        if (groundCheckLocked) return;
        
        // // 박스 캐스트로 플레이어의 아래 방향을 체크
        // // 레이 캐스트가 아닌 이유는
        // // 플레이어가 플랫폼 끝자락에 위치할 때 제대로 체크되지 않는 경우를 방지하기 위함
        // RaycastHit2D hit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, Vector2.down,
        //     0.02f, groundLayer);
        
        // GroundCheck 내부에서 BoxCast 대신에…
        RaycastHit2D hit = Physics2D.CapsuleCast(capsuleCollider.bounds.center,
                                                 capsuleCollider.size,
                                                 CapsuleDirection2D.Vertical,
                                                 0f,
                                                 Vector2.down,
                                                 0.02f,
                                                 groundLayer);
        
        // 점프 가능한 오브젝트에 닿은 동시에
        // 그 위치가 플레이어 기준으로 아래 방향이라면
        if(hit.collider != null && hit.normal.y > 0.7f)
        {
            //  isGround를 true로
            isGround = true;
        }
        else
        {
            // 위의 조건이 충족 상태가 아니라면 false로
            isGround = false;
        }

        // 애니메이터의 Ground 파라미터의 값을 isGround에 따라서 수정
        anim.SetBool(PlayerAnimHash.AnimGround, isGround);
    }
    
    /// <summary>
    /// 플레이어 이동 조작을 위한 입력 함수
    /// </summary>
    /// <param name="context">입력 값</param>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // 입력을 시작했거나 입력 중이라면
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
        {
            // 입력된 값을 moveDir에 적용
            moveDir = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // 입력이 없다면 곧바로 이동을 정지
            moveDir = Vector2.zero;
        }
    }
    
    /// <summary>
    /// 점프 키 입력 함수
    /// 키보드 입력 담당
    /// </summary>
    /// <param name="context">입력 값</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        // 점프 키 입력이 있다면(짧게라도)
        if (context.phase == InputActionPhase.Started)
        {
            // 실제 점프 동작 함수 호출
            Jump();
        }
    }

    /// <summary>
    /// 형태 변화 키 입력 함수
    /// 키보드 입력 담당
    /// </summary>
    /// <param name="context"></param>
    public void OnFormChange(InputAction.CallbackContext context)
    {
        // 충분한 입력이 없었거나(performed를 체크함으로...)
        // 땅에 닿은 상태가 아니거나
        // 조작이 불가능한 상태인 경우에는 return
        if (!context.performed || !isGround || !isControllable) return;
        var skillBtn = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
        switch (context.control.name)
        {
            case "1":
                skillBtn.OnSquirrel();
                break;
            case "2":
                skillBtn.OnDog();
                break;
            case "3":
                skillBtn.OnCat();
                break;
            case "4":
                skillBtn.OnHide();
                break;
        }
    }

    /// <summary>
    /// 실제 플레이어 이동 동작 함수
    /// </summary>
    public void Move()
    {
        // 조작이 불가능한 상태라면 동작 X
        if (!isControllable) return;

        if (Mathf.Abs(moveDir.x) > 0.1f)
        // 좌우 입력 값이 있고
        // 나뭇잎 트램펄린을 이용하는 중이 아니라면
        {
            if (TryDetectBox(moveDir))
            {
                IPusher pusher = player.FormControl;
                float pushPower = pusher.GetPushPower();
                float objWeight = this.objWeight.GetWeight();
                basePushPower = (pushPower / objWeight) * moveSpeed;
                //미는 속도 = 미는 힘 / 무게 * 이동속도
                basePushPower = Mathf.Min(basePushPower, moveSpeed);
                // 미는 속도의 최대 이동속도 이상을 초과할 수 없도록

                rigid.velocity = new Vector2(moveDir.x * basePushPower, rigid.velocity.y);   
            }
            else
            {
                // 상자를 미는 상태가 아니라면 이동속도와 입력 방향에 맞춰 이동
                rigid.velocity = new Vector2(moveDir.x * moveSpeed, rigid.velocity.y);
            }
            // FlipControl 함수에 플레이어 이동 방향을 전달
            player.FormControl.FlipControl(moveDir);
        }
        else
        {
            // 좌우 입력이 없다면 정지
            rigid.velocity = new Vector2(0f, rigid.velocity.y);
        }
    }

    /// <summary>
    /// 사다리 타기 함수
    /// </summary>
    public void ClimbLadder()
    {
        // 상하 입력이 있다면
        if(Mathf.Abs(moveDir.y) > 0.1f)
        {
            // 사다리를 따라서 상하 이동
            rigid.velocity = new Vector2(0f, moveDir.y * moveSpeed);
        }
        else
        {
            // 입력이 없다면 정지
            rigid.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 실제 플레이어의 점프 동작 함수
    /// </summary>
    public void Jump()
    {
        // 동작 불가 상태라면 return
        if (!isControllable) return;

        // 점프 가능한 레이어의 오브젝트에 발(오브젝트의 아래 방향)이 닿은 상태라면
        if (isGround)
        {
            // 플레이어가 점프로 위를 향해 뛰어오르지 않았을 때
            // 0으로 하면 단순 이동 중에도 점프가 되지 않음
            // 평지라고 해도 아주 조금씩 y값은 변화중
            if (rigid.velocity.y <= 0.2f)
            {
                // 플레이어의 상태를 점프 상태로 전환
                player.StateMachine.ChangeState(player.StateMachine.Factory.GetPlayerState(PlayerStateType.Jump));
            }
        }
        else if(player.StateMachine.CurState == 
            player.StateMachine.Factory.GetPlayerState(PlayerStateType.WallCling))
        {
            // 플레이어가 고양이 형태로 벽에 붙은 상태라면 벽 점프 상태로 전환
            player.StateMachine.ChangeState(player.StateMachine.Factory.GetPlayerState(PlayerStateType.WallJump));
        }
    }

    public float GetWeight()
    {
        return Managers.Instance.GameManager.Player.FormControl.GetWeight();
    }
    
    public void StartLeafJump(Vector2 dropPosition,float jumpPower)
    {
        StopAllCoroutines();
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 1f;

        Vector2 impulse = dropPosition * jumpPower * rigid.mass; 
        rigid.AddForce(impulse, ForceMode2D.Impulse);

        StartCoroutine(ApplyGravity());
    }

    private IEnumerator ApplyGravity()
    {
        yield return new WaitUntil(() => rigid.velocity.y <= 0f);

        rigid.gravityScale = 2f;
    }

    public bool TryDetectBox(Vector2 dir)
    {
        Vector2 origin = (Vector2)capsuleCollider.bounds.center
        + Vector2.right * (Mathf.Sign(dir.x) * (capsuleCollider.bounds.extents.x + 0.01f));

        Vector2 originalSize = capsuleCollider.bounds.size*0.8f;
        Vector2 size = new Vector2(originalSize.x * 0.2f, originalSize.y);

        Collider2D hit = Physics2D.OverlapBox(
            origin, size, 0f,
            pushableLayer
             );

        if (hit != null)
        {
            if (hit.TryGetComponent<IWeightable>(out var weight))
            // IWeightable이 붙은 컴포넌트인지 확인하고, 맞으면 True반환과 무게를 반환        
            {
                objWeight = weight;
                objRigid = hit.attachedRigidbody;
                // Collider가 붙어있는 Rigidbody2D를 가져오고
                return true;
            }
        }

        objWeight = null;
        objRigid = null;
        return false;
    }

    //sprite 이미지에 맞춰서 콜라이더 생성
    public void SetCollider(CapsuleDirection2D direct)
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Bounds bounds = spriteRenderer.bounds;

        // bounds는 world size이므로, 로컬 좌표로 환산 필요
        Vector2 size = transform.InverseTransformVector(bounds.size);
        Vector2 offset = transform.InverseTransformPoint(bounds.center);

        capsuleCollider.direction = direct;
        capsuleCollider.size = size;
        capsuleCollider.offset = offset;
    }

    /// <summary>
    /// 플레이어 조작 잠금 함수
    /// </summary>
    public void LockPlayer()
    {
        // 걸으면서 대화를 할 수 없도록 하기 위해서 Idle 상태로 변경
        player.StateMachine.ChangeState(player.StateMachine.Factory.GetPlayerState(PlayerStateType.Idle)); // Idle 상태로 변경
        isControllable = false;
        rigid.velocity = Vector2.zero;
    }
    
    /// <summary>
    /// 플레이어 조작 잠금 해제 함수
    /// </summary>
    public void UnlockPlayer()
    {
        if (Managers.Instance.CutSceneManager.IsCutScenePlaying) return;
        isControllable = true;
    }

    private void OnDestroy()
    {
        Managers.Instance.CutSceneManager.OnCutSceneStart -= LockPlayer;
        Managers.Instance.CutSceneManager.OnCutSceneEnd -= UnlockPlayer;
        
        Managers.Instance.DialogueManager.OnDialogStart -= LockPlayer;
        Managers.Instance.DialogueManager.OnDialogEnd -= UnlockPlayer;
    }

    public void ResetPlayer()
    {
        player.transform.position = playerBasePos;
    }

    public void BackMove()
    {
        LockPlayer();
        // 충돌 시 뒤로 밀어주기
        Vector2 vector2 = new Vector2((-moveDir.x * moveSpeed), rigid.velocity.y);
        rigid.velocity = vector2;

        Invoke("BackMoveStop", 0.5f);
    }

    private void BackMoveStop()
    {
        var popup = Managers.Instance.UIManager.Show<WarningPopup>(WarningType.BackMove);
        popup.SetScreenPosition(player.transform.position + new Vector3(0, 1.5f, 0));

        UnlockPlayer();
        rigid.velocity = Vector2.zero;

    }
}