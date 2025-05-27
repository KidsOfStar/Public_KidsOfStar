using UnityEngine;

/// <summary>
/// 플레이어와의 상호작용을 통한 퍼즐 진입을 담당
/// </summary>
public class WirePuzzleTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("튜토리얼을 띄울 트리거인지 여부")] private bool isTutorialDoor = false;
    [SerializeField, Tooltip("연결된 퍼즐 데이터")] private WirePuzzleData puzzleData;
    [SerializeField, Tooltip("느낌표 표시 프리팹")] private GameObject exclamationInstance;
    // 느낌표 표시 오브젝트의 스프라이트 렌더러
    private SpriteRenderer exclamationRenderer;
    [SerializeField, Tooltip("트리거가 작동할 ChapterProgress 값")]
    private int requireProgress;
    [SerializeField, Tooltip("이 트리거의 퍼즐이 클리어 되면 작동하는 엘리베이터")]
    private Elevator lockedElevator;
    public Elevator LockedElevator { get { return lockedElevator; } }

    // 상호 작용 버튼
    private SkillBTN skillBtn;
    // 시퀀스 번호
    [SerializeField] private int sequenceIndex;
    public int SequenceIndex => sequenceIndex;
    // 플레이어가 트리거 안에 있는지 여부
    private bool hasPlayer = false;
    // 튜터리얼 트리거인 경우, 이미 튜토리얼을 보였는지 여부
    private bool tutorialShown = false;
    // 퍼즐이 이미 작동되었는지 여부
    private bool triggered = false;

    private void Awake()
    {
        exclamationRenderer = exclamationInstance.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // 현재 진행도가 조건과 맞는 경우 느낌표 활성화
        bool show = Managers.Instance.GameManager.ChapterProgress == requireProgress;
        exclamationRenderer.enabled = show;

        // 상호 작용 버튼 연결
        skillBtn = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
        // 진행도 갱신 이벤트 구독
        Managers.Instance.GameManager.OnProgressUpdated += UpdateExclamation;
    }

    // 느낌표 스프라이트 비활성화
    public void DisableExclamation()
    {
        exclamationRenderer.enabled = false;
    }

    // 느낌표 표시 갱신
    private void UpdateExclamation()
    {
        bool show = Managers.Instance.GameManager.ChapterProgress == requireProgress;
        exclamationRenderer.enabled = show;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 실행된 상태 || 현재 진행도와 맞지 않는 진행도 조건
        if (triggered || Managers.Instance.GameManager.ChapterProgress != requireProgress)
            return;

        // 닿은 오브젝트의 태그가 Player일 때
        if(collision.CompareTag("Player"))
        {
            // 조건이 이미 충족된 상태 이면 return
            if (hasPlayer) return;

            // 현재 형태가 인간인지 확인
            var formControl = Managers.Instance.GameManager.Player.FormControl;
            bool isHuman = formControl.ReturnCurFormType() == PlayerFormType.Human;

            // 사람 형태가 아닐 때
            if(!isHuman)
            {
                // 사람 형태 변신이 필요하다는 경고
                Managers.Instance.UIManager.Show<WarningPopup>(WarningType.Squirrel);
                return;
            }

            hasPlayer = true;
        }
        else
        {
            return;
        }

        TryEnableInteraction();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggered) return;

        if(collision.CompareTag("Player"))
        {
            hasPlayer = false;
        }
        else
        {
            return;
        }

        HideInteraction();
    }

    // 상호 작용 버튼을 보여줄 상태인지 체크하고 클릭 이벤트 연결
    private void TryEnableInteraction()
    {
        if(hasPlayer)
        {
            skillBtn.ShowInteractionButton(true);
            skillBtn.OnInteractBtnClick -= OnPuzzleButtonPressed;
            skillBtn.OnInteractBtnClick += OnPuzzleButtonPressed;
        }
    }

    // 상호 작용 버튼 클릭 시 실행
    private void OnPuzzleButtonPressed()
    {
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.Communication);
        TryStartPuzzle();
    }

    // 상호 작용 버튼 숨김 및 이벤트 제거
    private void HideInteraction()
    {
        skillBtn.ShowInteractionButton(false);
        skillBtn.OnInteractBtnClick -= OnPuzzleButtonPressed;
    }

    // 퍼즐 시작 처리
    private void TryStartPuzzle()
    {
        // 이미 퍼즐이 시작된 상태라면 return
        if (triggered) return;
        triggered = true;

        HideInteraction();

        // 튜토리얼 트리거 && 첫 진행도 퍼즐 && 튜토리얼을 아직 보여주지 않은 경우
        if(isTutorialDoor
            &&requireProgress == Managers.Instance.GameManager.ChapterProgress
            && sequenceIndex == 0 && !tutorialShown)
        {
            tutorialShown = true;
            var popup = Managers.Instance.UIManager.Show<TutorialPopup>(1);
            popup.OnClosed += () =>
            {
                Managers.Instance.UIManager.Show<WirePuzzlePopup>(puzzleData, sequenceIndex);
            };
            return;
        }

        // 퍼즐 팝업 열기
        Managers.Instance.UIManager.Show<WirePuzzlePopup>(puzzleData, sequenceIndex);
    }

    // 퍼즐 트리거 초기화
    public void ResetTrigger()
    {
        triggered = false;
        HideInteraction();

        if (hasPlayer)
            TryEnableInteraction();
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        Managers.Instance.GameManager.OnProgressUpdated -= UpdateExclamation;
    }
}
