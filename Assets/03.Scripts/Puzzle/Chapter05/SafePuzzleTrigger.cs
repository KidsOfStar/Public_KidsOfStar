using UnityEngine;
using UnityEngine.UI;

public class SafePuzzleTrigger : MonoBehaviour
{
    [Header("상호작용 이펙트")]
    public SceneType sceneType;
    [SerializeField] private GameObject exclamationInstance;

    //private SpriteRenderer exclamationRenderer;

    [SerializeField] private GameObject bubbleTextPrefab;
    private GameObject bubbleTextInstance; // 문 위에 생성된 프리팹 인스턴스

    private SkillBTN skillBTN;
    [Header("튜토리얼 체크")]
    [SerializeField] private bool isTutorial = true;

    // 동물 폼
    [SerializeField] private PlayerFormType dangerFormMask;

    [Header("현재 진행도와 금고 번호")]
    [SerializeField] private int curChapterProgress; // 현재 진행도
    public int safeNumber; // 각 씬의 금고 번호

    [Header("필요 시 추가 상호작용 요소")]
    public Door door;

    private void Start()
    {
        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;

    }
    public void init()
    {
        // 진행도 갱신 이벤트 구독
        Managers.Instance.GameManager.OnProgressUpdated += DisableExclamation;

        Debug.Log($"{Managers.Instance.GameManager.ChapterProgress}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (door.isDoorOpen || Managers.Instance.GameManager.ChapterProgress != curChapterProgress)
            return;

        if (collision.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(true); // 상호작용 버튼 활성화
            skillBTN.OnInteractBtnClick += OnInteraction; // 상호작용 버튼 클릭 이벤트 등록
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HideInteraction();

            if (bubbleTextInstance != null)
            {
                Destroy(bubbleTextInstance);
                bubbleTextInstance = null;
            }
        }
    }

    private void OnInteraction()
    {
        var player = Managers.Instance.GameManager.Player;
        var currentForm = player?.FormControl?.CurFormData;

        if(currentForm == null) return;

        if ((dangerFormMask & currentForm.playerFormType) != 0)
        {
            OntextBubbleText();
            //Managers.Instance.UIManager.Show<WarningPopup>(
            //    WarningType.Squirrel
            //    );
        }
        else
            TryStartPuzzle();
    }

    private void TryStartPuzzle()
    {
        if (isTutorial)
        {
            isTutorial = false;
            var popup = Managers.Instance.UIManager.Show<TutorialPopup>(4);
            popup.OnClosed += () =>
            {
                Debug.Log("튜토리얼 후 퍼즐 시작");

                var safePopup = Managers.Instance.UIManager.Show<SafePopup>();
                safePopup.Opened(sceneType);  // 안전한 폼일 경우 팝업 표시
                safePopup.safePuzzle.SetSafeNumber(safeNumber);
            };
            return;
        }
        Debug.Log("퍼즐 시작");
        // 튜토리얼 보여주고 시작
        var safePopup = Managers.Instance.UIManager.Show<SafePopup>(); // 안전한 폼일 경우 팝업 표시
        safePopup.Opened(sceneType);  // 안전한 폼일 경우 팝업 표시
        safePopup.safePuzzle.SetSafeNumber(safeNumber);

    }

    private void OntextBubbleText()
    {
        if (bubbleTextInstance == null && bubbleTextPrefab != null)
        {
            bubbleTextInstance = Instantiate(bubbleTextPrefab, this.transform);
            bubbleTextInstance.transform.localPosition = new Vector3(0, 2f, 0);

            var bubbleText = bubbleTextInstance.GetComponentInChildren<DoorPopup>();

            if (bubbleText != null)
            {
                bubbleText.SetText("세심하게 만져야 한다. 다른 방법이 없을까?");
            }
        }
        else if (bubbleTextInstance != null)
        {
            Destroy(bubbleTextInstance);
            bubbleTextInstance = null;
        }
    }

    private void HideInteraction()
    {
        skillBTN.ShowInteractionButton(false);
        skillBTN.OnInteractBtnClick -= OnInteraction;
    }

    // 게임 클리어시 비활성화
    public void DisableExclamation()
    {
        Debug.Log("실행 중");
        Debug.Log($"현재 진행도: {Managers.Instance.GameManager.ChapterProgress}, 설정된 진행도: {curChapterProgress}");
        if (Managers.Instance.GameManager.ChapterProgress != curChapterProgress)
            exclamationInstance.SetActive(false);
        if (Managers.Instance.GameManager.ChapterProgress > curChapterProgress)
            door.isDoorOpen = true; // 문 열기
    }

    private void OnDestroy()
    {
        Managers.Instance.GameManager.OnProgressUpdated -= DisableExclamation; // 이벤트 구독 해제
    }
}
