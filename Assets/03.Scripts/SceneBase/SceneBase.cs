using System;
using UnityEngine;

// 풀 생성, npc 넘겨주기 등 씬 초기화에 필요한 작업들을 담당
// 씬에는 반드시 SceneBase를 상속받은 베이스가 있어야 함
// 씬 고유 초기화 작업은 InitSceneExtra() 메서드에서 수행
public abstract class SceneBase : MonoBehaviour
{
    [Header("Chapter")]
    [SerializeField] private ChapterType currentChapter;
    [SerializeField] private bool existRequiredDialog = true;
    [SerializeField] protected bool isFirstTime = true;
    [SerializeField, TextArea] private string introText;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab; // TODO: 리소스 로드 할지?
    [SerializeField] protected PlayerFormType playerStartForm;
    [SerializeField] protected Transform playerSpawnPosition;

    [Header("NPCs"), Tooltip("중복해서 넣지 않도록 주의해주세요")]
    [SerializeField] private SceneNpc[] speakers;
    [SerializeField] private InteractObject[] objects;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Funnel")]
    [SerializeField] private FunnelData funnelData;

    [Header("Player Position")]
    [Tooltip("컷신 이후 플레이어 위치를 잡는 부모오브젝트")]
    [SerializeField] private PlayerSpawnPointer spawnPointer;
    [SerializeField] private ScrollingBackGround scrollingBackGround;

    private Action onCutSceneEndHandler;

    private void Awake()
    {
        // 매니저들 초기화
        InitManagers();

        CreatePool();

        // 로딩중인 씬 매니저에게 씬이 활성화 되었음을 알림
        Managers.Instance.SceneLoadManager.IsSceneLoadComplete = true;

#if UNITY_EDITOR
        Managers.Instance.LoadTestScene = false;
#endif

        // 씬이 로드된 후에 플레이어를 스폰
        SpawnPlayer();

        // 게임 매니저에 현재 챕터를 설정
        InitSceneBase();

        // 플레이어 스폰 후 카메라 컨트롤러의 타겟 설정
        InitCameraController();

        // 필수 UI 표시
        ShowRequiredUI();

        // 씬 고유 초기화 작업
        // 챕터별 컷신이 필요한 경우 컷씬 재생 이후 Bgm, Intro 등이 재생되도록 콜백을 등록
        InitSceneExtra(CutSceneEndCallback);

        // 컷신 종료 후 이동 할 플레이어 위치 정보 설정
        if (spawnPointer) spawnPointer.Init();

        InitBg();

        SendFunnel();
    }

    private void InitManagers()
    {
        Managers.Instance.GameManager.SetCamera(mainCamera);
        Managers.Instance.OnSceneLoaded();
        Managers.Instance.DialogueManager.InitSceneNPcs(speakers);
    }

    protected virtual void CreatePool()
    {
        // 필수 대화 아이콘 풀 생성
        if (existRequiredDialog)
        {
            const string path = Define.uiPath + Define.requiredIconKey;
            var prefab = Managers.Instance.ResourceManager.Load<GameObject>(path);
            Managers.Instance.PoolManager.CreatePool(prefab, 5);
        }
    }

    private void SpawnPlayer()
    {
        var gameManager = Managers.Instance.GameManager;
        Vector3 playerPosition = gameManager.IsNewGame ? playerSpawnPosition.position : gameManager.PlayerPosition;

        GameObject playerObj = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        Player player = playerObj.GetComponent<Player>();

        var currentForm = Managers.Instance.GameManager.CurrentForm;
        if (Managers.Instance.GameManager.IsNewGame)
            player.Init(playerStartForm == 0 ? currentForm : playerStartForm);
        else
            player.Init(currentForm);

        Managers.Instance.GameManager.SetPlayer(player);
        Managers.Instance.DialogueManager.SetPlayerSpeaker(player);

        onCutSceneEndHandler += () =>
        {
            playerObj.transform.position = playerSpawnPosition.position;
            playerObj.transform.rotation = playerSpawnPosition.rotation;
        };
        Managers.Instance.CutSceneManager.OnCutSceneEnd += onCutSceneEndHandler;
    }

    private void InitSceneBase()
    {
        Managers.Instance.GameManager.SetChapter(currentChapter);

        // 챕터 변경 후 Npc 초기화
        InitNpc();
        InitObjects();

        if (Managers.Instance.GameManager.IsNewGame)
            Managers.Instance.GameManager.ResetProgress();
        else
        {
            isFirstTime = false;
            Managers.Instance.GameManager.SetLoadedProgress();
        }
    }

    private void InitCameraController()
    {
        if (mainCamera.TryGetComponent(out CameraController cameraController))
            cameraController.Init();
        else
            EditorLog.LogError("SceneBase : CameraController not found on the main camera.");
    }

    private void ShowRequiredUI()
    {
        Managers.Instance.UIManager.Show<UIJoystick>();
        Managers.Instance.UIManager.Show<PlayerBtn>().Init();
    }

    // 씬 내에서 TriggerEnter로 진행도를 업데이트할 때 사용
    public void UpdateProgress()
    {
        Managers.Instance.GameManager.UpdateProgress();
    }

    protected void PlayChapterIntro(Action callback = null)
    {
        var intro = Managers.Instance.UIManager.Show<UIChapterIntro>();
        StartCoroutine(intro.IntroCoroutine(isFirstTime, introText, callback));
    }

    private void InitNpc()
    {
        for (int i = 0; i < speakers.Length; i++)
        {
            var speaker = speakers[i];
            speaker.Init();

            // ProgressNpcPlacer가 있다면 초기화
            if (speaker.TryGetComponent(out ProgressNpcPlacer placer))
                placer.Init();
        }
    }

    private void InitObjects()
    {
        if (objects == null || objects.Length == 0)
            return;

        for (int i = 0; i < objects.Length; i++)
        {
            var obj = objects[i];
            obj.Init();

            // ProgressNpcPlacer가 있다면 초기화
            if (obj.TryGetComponent(out ProgressNpcPlacer placer))
                placer.Init();
        }
    }

    private void InitBg()
    {
        if (scrollingBackGround != null)
            scrollingBackGround.Initialized(mainCamera.transform);
    }

    private void SendFunnel()
    {
        if (funnelData?.dialogStartFunnel != null)
        {
            Managers.Instance.DialogueManager.OnDialogStepStart += SendDialogStartFunnel;
        }
        
        if (funnelData?.dialogEndFunnel != null)
        {
            Managers.Instance.DialogueManager.OnDialogStepEnd += SendDialogEndFunnel;
        }
        
        if (funnelData?.cutSceneStartFunnel != null)
        {
            Managers.Instance.CutSceneManager.OnCutSceneStart += SendCutSceneStartFunnel;
        }
    }

    private void SendDialogStartFunnel(int dialogIndex)
    {
        if (funnelData?.dialogStartFunnel == null) return;

        var dict = funnelData.GetDialogStartDict();
        foreach (var pair in dict)
        {
            if (pair.Key == dialogIndex)
            {
                Managers.Instance.AnalyticsManager.SendFunnel(pair.Value.ToString());
            }
        }
    }

    private void SendDialogEndFunnel(int dialogIndex)
    {
        if (funnelData?.dialogEndFunnel == null) return;

        var dict = funnelData.GetDialogEndDict();
        foreach (var pair in dict)
        {
            if (pair.Key == dialogIndex)
            {
                Managers.Instance.AnalyticsManager.SendFunnel(pair.Value.ToString());
            }
        }
    }

    private void SendCutSceneStartFunnel()
    {
        if (funnelData?.cutSceneStartFunnel == null) return;

        var dict = funnelData.GetCutSceneStartDict();
        foreach (var pair in dict)
        {
            if (pair.Key == Managers.Instance.CutSceneManager.CurrentCutSceneName)
            {
                Managers.Instance.AnalyticsManager.SendFunnel(pair.Value.ToString());
            }
        }
    }

    protected abstract void InitSceneExtra(Action callback);

    protected abstract void CutSceneEndCallback();

    protected virtual void OnDestroy()
    {
        // 씬이 파괴될 때 반드시 구독 해제
        if (onCutSceneEndHandler != null)
        {
            Managers.Instance.CutSceneManager.OnCutSceneEnd -= onCutSceneEndHandler;
            onCutSceneEndHandler = null;
        }
        
        Managers.Instance.DialogueManager.OnDialogStepStart -= SendDialogStartFunnel;
        Managers.Instance.DialogueManager.OnDialogStepEnd -= SendDialogEndFunnel;
        Managers.Instance.CutSceneManager.OnCutSceneStart -= SendCutSceneStartFunnel;
    }
}