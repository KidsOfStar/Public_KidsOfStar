using UnityEngine;

public class Player : MonoBehaviour, IDialogSpeaker
{
    // 플레이어 컨트롤러 스크립트(주로 입력을 받고 실제 동작을 하는 기능)
    private PlayerController controller;
    public PlayerController Controller { get { return controller; } }

    // 플레이어 형태 변화 스크립트(플레이어의 형태 변화 스킬을 관리)
    private PlayerFormController formControl;
    public PlayerFormController FormControl { get { return formControl; } }

    // 플레이어 상태 머신 스크립트
    private PlayerStateMachine stateMachine;
    public PlayerStateMachine StateMachine { get { return stateMachine; } }

    [SerializeField, Tooltip("말풍선 위치")] private Transform bubblePosition;

    /// <summary>
    /// 플레이어를 초기화 하기 위한 함수
    /// </summary>
    /// <param name="formType">플레이어가 처음에 취할 형태</param>
    public void Init(PlayerFormType formType)
    {
        // MonoBehavior를 상속한 각 스크립트를 가져오기
        controller = GetComponent<PlayerController>();
        formControl = GetComponent<PlayerFormController>();
        stateMachine = GetComponent<PlayerStateMachine>();

        // 가져온 스크립트를 초기화
        controller.Init(this);
        formControl.Init(this);
        stateMachine.Init(this);

        // 플레이어 초기 형태 적용
        // 따로 명시된 형태가 없으면 인간의 형태로
        formControl.CutSceneFormChange(formType);
    }

    /// <summary>
    /// 캐릭터 타입 반환 함수
    /// </summary>
    /// <returns>캐릭터 타입</returns>
    public CharacterType GetCharacterType()
    {
        return CharacterType.Dolmengee;
    }

    /// <summary>
    /// 말풍선이 위치할 좌표 반환 함수
    /// </summary>
    /// <returns>말풍선 위치</returns>
    public Transform GetBubbleTr()
    {
        return bubblePosition;
    }
}