using UnityEngine;

public class TreePuzzleTrigger : MonoBehaviour
{
    private bool tutorialShown = false;
    private bool triggered = false;
    private bool hasPlayer = false;
    private bool hasBox = false;

    [Header("튜토리얼 문인지 체크")]
    [SerializeField] private bool isTutorialDoor = false;

    private SkillBTN skillBTN;

    [SerializeField] private TreePuzzleData puzzleData;
    [SerializeField] private int sequenceIndex;
    public int SequenceIndex => sequenceIndex;

    [SerializeField] private GameObject exclamationInstance;
    private SpriteRenderer exclamationRenderer;

    [Header("트리거가 켜질 ChapterProgress 값")]
    [SerializeField] private int requiredProgress;

    [SerializeField] private string puzzleLayerName = "PuzzleDoor";
    private int puzzleLayer;

    private void Awake()
    {
        puzzleLayer = LayerMask.NameToLayer(puzzleLayerName);

        if (gameObject.layer != puzzleLayer)
        {
            enabled = false;
            return;
        }

        // SpriteRenderer 컴포넌트 가져오기
        exclamationRenderer = exclamationInstance.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        bool show = Managers.Instance.GameManager.ChapterProgress == requiredProgress;
        exclamationRenderer.enabled = show;

        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;

       Managers.Instance.GameManager.OnProgressUpdated += UpdateExclamation;
    }

    // 게임 클리어시 비활성화
    public void DisableExclamation()
    {
        exclamationRenderer.enabled = false;
    }

    private void UpdateExclamation()
    {
        int progress = Managers.Instance.GameManager.ChapterProgress;
        exclamationRenderer.enabled = (progress == requiredProgress);

        if (progress > requiredProgress)
        {
            HideInteraction();          
            Managers.Instance.GameManager.OnProgressUpdated -= UpdateExclamation;
            this.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.layer != LayerMask.NameToLayer("PuzzleDoor"))
            return;

        if (triggered || Managers.Instance.GameManager.ChapterProgress != requiredProgress) 
            return;

        if (collision.CompareTag("Player"))
        {
            if (hasPlayer) return;
            hasPlayer = true;

            // 다람쥐 형태 검사
            var formControl = Managers.Instance.GameManager.Player.FormControl;
            bool isSquirrel = formControl.ReturnCurFormType() == PlayerFormType.Squirrel;

            if (isSquirrel && !hasBox)
            {
                // 다람쥐 & 상자 없음 → 두 문구를 순차적으로
                Managers.Instance.UIManager.Show<WarningPopup>(
                    WarningType.Squirrel,
                    WarningType.BoxMissing
                );
                return;
            }
            else if (isSquirrel)
            {
                // 다람쥐지만 상자는 들고 있는 경우 → 한 문구만
                Managers.Instance.UIManager.Show<WarningPopup>(
                    WarningType.Squirrel
                );
                return;
            }

            if (!hasBox)
            {
                // 다람쥐가 아니지만, 상자가 없는 경우
                Managers.Instance.UIManager.Show<WarningPopup>(
                    WarningType.BoxMissing
                );
            }
        }
        else if (collision.CompareTag("Box"))
        {
            if (hasBox) return;
            hasBox = true;
        }
        else
        {
            return;
        }

        TryEnableInteraction();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Managers.Instance.GameManager.ChapterProgress != requiredProgress || triggered)
            return;

        if (collision.CompareTag("Player"))
            hasPlayer = false;

        else if (collision.CompareTag("Box"))
            hasBox = false;

        else
            return;

        HideInteraction();
    }

    private void TryEnableInteraction()
    {
        if (hasPlayer && hasBox) 
        {
            skillBTN.ShowInteractionButton(true);
            skillBTN.OnInteractBtnClick -= OnPuzzleButtonPressed;
            skillBTN.OnInteractBtnClick += OnPuzzleButtonPressed;
        }
    }

    private void TryStartPuzzle()
    {
        if (triggered) return;
        triggered = true;

        HideInteraction();

        if(isTutorialDoor 
           && requiredProgress == Managers.Instance.GameManager.ChapterProgress 
           && sequenceIndex ==0 
           && !tutorialShown)
        {
            tutorialShown = true;
            var popup = Managers.Instance.UIManager.Show<TutorialPopup>(0);
            popup.OnClosed += () =>
            {
                Managers.Instance.UIManager.Show<TreePuzzlePopup>(puzzleData, sequenceIndex);
            };
            return;

        }
        Managers.Instance.UIManager.Show<TreePuzzlePopup>(puzzleData, sequenceIndex);
    }

    private void HideInteraction()
    {
        skillBTN.ShowInteractionButton(false);
        skillBTN.OnInteractBtnClick -= OnPuzzleButtonPressed;
    }

    private void OnPuzzleButtonPressed()
    {
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.Communication);
        TryStartPuzzle();
    }

    public void ResetTrigger()
    {
        triggered = false;

        HideInteraction();

        if (hasPlayer && hasBox)
            TryEnableInteraction();
    }

    private void OnDestroy()
    {
        Managers.Instance.GameManager.OnProgressUpdated -= UpdateExclamation;
    }
}
