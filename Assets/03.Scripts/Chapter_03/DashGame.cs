using Cinemachine;
using System.Collections;
using UnityEngine;

public class DashGame : MonoBehaviour
{
    [Header("Game Settings")]
    public StopWatch stopWatch;
    public CountDownPopup countDownPopup;
    public PlayerController playerController;

    public CinemachineVirtualCamera virtualCamera;  // 가상 카메라
    public CinemachineDollyCart trackedDolly;       // 카메라 이동 경로


    public CharacterType characterType; // 캐릭터 타입
    
    public float cameraMoveSpeed;  // 카메라 이동 속도

    public float playerSpeed; // 플레이어 속도
    public float playerSpeedMultiplier = 3f; // 플레이어 속도 배율
    public bool isGameStarted = false;
    public bool showDialog = false;

    
    private SkillBTN skillBTN; // 스킬 버튼 UI
    [SerializeField] private GameObject TestGameBlock;  // 끌날 떄
    [SerializeField] private GameObject StartGameBlock; // 게임 시작 블록

    [Header("AnalyticsManager")]
    [SerializeField] private DeadIine deadIine;     // 데드라인

    public void Setting()
    {
        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel; // 스킬 버튼 UI 가져오기

        playerController = Managers.Instance.GameManager.Player.Controller;
        playerSpeed = playerController.MoveSpeed; // 플레이어 속도 초기화

        countDownPopup = Managers.Instance.UIManager.Show<CountDownPopup>();
        Managers.Instance.UIManager.Hide<CountDownPopup>(); // 카운트다운 팝업 숨김

        stopWatch = Managers.Instance.UIManager.Show<StopWatch>();
        Managers.Instance.UIManager.Hide<StopWatch>(); // 스탑워치 숨김
    }

    public void StartGame()
    {
        if (isGameStarted) return;
        isGameStarted = true;

        // AnalyticsManager 게임 시도
        Managers.Instance.AnalyticsManager.TryCount++; // 시도 횟수 증가

        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.WithDogsRun);

        StartCoroutine(GameIntroSequence()); // 전체 흐름 관리

        Managers.Instance.AnalyticsManager.SendFunnel("20");
    }

    private IEnumerator GameIntroSequence()
    {
        yield return null;
        skillBTN.ShowInteractionButton(false); // 스킬 버튼 비활성화

        yield return null; // 한 프레임 대기 유예하여 언락을 실행 다음에 락이 되도록 하기 위해 작성함
        playerController.LockPlayer(); // 플레이어 잠금

        yield return StartCoroutine(VirtualCameraMove()); // 카메라 이동 끝날 때까지 대기

        yield return new WaitForSeconds(1f); // 1초 대기

        // 카운트다운 + 게임 시작 루틴
        Managers.Instance.UIManager.Show<CountDownPopup>();
        countDownPopup.CountDownStart();

        Managers.Instance.UIManager.Show<StopWatch>();

        yield return StartCoroutine(StartGame(5f)); // 5초 카운트다운 후 게임 시작

        StartGameBlock.SetActive(false); // 게임 시작 블록 비활성화

    }

    private IEnumerator VirtualCameraMove()
    {
        virtualCamera.gameObject.SetActive(true); // 가상 카메라 활성화
        virtualCamera.Priority = 20; // 가상 카메라 우선순위 변경

        while (trackedDolly.m_Position < trackedDolly.m_Path.PathLength) // 카메라 이동 조건
        {
            trackedDolly.m_Position += cameraMoveSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
            if(trackedDolly.m_Position == trackedDolly.m_Path.PathLength) // 카메라 이동 완료
            {
                trackedDolly.m_Position = trackedDolly.m_Path.PathLength; // 카메라 위치 고정
                //yield return new WaitForSeconds(1f); // 1초 대기
                break; // 루프 종료
            }
        }
        yield return new WaitForSeconds(1f); // 마지막 모습 보이기

        virtualCamera.Priority = -20; // 가상 카메라 우선순위 변경
        virtualCamera.gameObject.SetActive(false); // 가상 카메라 비활성화
    }

    private IEnumerator StartGame(float delay)
    {
        yield return null;  // 한 프레임 대기 유예하여 언락을 실행 다음에 락이 되도록 하기 위해 작성함
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.WithDogsRun);

        yield return new WaitForSeconds(delay); // 카운트다운 대기
        stopWatch.OnStartWatch();   // 스탑워치 시작
        stopWatch.StartTime();      // 스탑워치 시간 시작

        playerController.UnlockPlayer(); // 플레이어 잠금
        playerController.MoveSpeed = playerSpeed * playerSpeedMultiplier; // 플레이어 속도 초기화 (1.5배 증가)

        yield return null;
        virtualCamera.Priority = -10; // 가상 카메라 우선순위 변경
    }

    public void EndGame(CharacterType npcType)
    {
        if (!isGameStarted) return;

        stopWatch.OnStopWatch();

        playerController.LockPlayer(); // 플레이어 잠금
        playerController.MoveSpeed = playerSpeed; // 플레이어 속도 초기화 (원래 속도로 복구)

        float clearTime = stopWatch.recodeTime;

        characterType = npcType; // 캐릭터 타입 설정
        ShowDialogueResult(); // 대사 출력
        showDialog = true;

        var resultPopup = Managers.Instance.UIManager.Get<DashGameResultPopup>();
        resultPopup.OnDialogEnd -= DialogEnd;
        resultPopup.OnDialogEnd += DialogEnd;

        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.WithDogs);
        Managers.Instance.UIManager.Hide<StopWatch>(); // 스탑워치 표시
        Managers.Instance.UIManager.Hide<CountDownPopup>(); // 카운트다운 팝업 숨김

        TestGameBlock.SetActive(false); // 테스트 게임 블록 비활성화

        var analyticsManager = Managers.Instance.AnalyticsManager;

        analyticsManager.RecordChapterEvent("Chapter3RunningPuzzle",
            ("ClearTime", clearTime), // 클리어 시간
            ("ChallengeCount", analyticsManager.TryCount), // 몇 번만에 시도 횟수
            ("FallPosition", deadIine.playerPosX) // 낙하 횟수
        );
        Managers.Instance.AnalyticsManager.TryCount = 0; // 시도 횟수 초기화


        // **게임 종료 후에도 버튼 유지**
        //skillBTN.ShowInteractionButton(true);
    }

    private void DialogEnd()
    {
        showDialog = false;
    }

    private void Update()
    {
        if (!showDialog) return;
        
        if (Input.GetMouseButtonDown(0))
            ShowDialogueResult();
    }

    private void ShowDialogueResult()
    {
        // 메인 테이블이랑 SO파일 대사 합쳐서 관리하기
        // 이미 DashGameResultPopup이 열려 있다면, 다음 대사 출력 시도
        if (Managers.Instance.UIManager.Get<DashGameResultPopup>())
        {
            Managers.Instance.UIManager.Get<DashGameResultPopup>().OnClickDialogue();
        }
        else
        {
            // 1분 30초 미만일 때 대사 출력
            if (stopWatch.recodeTime < 90f)
            {
                // 1f는 내부에서 90f 기준 대사를 가져오는 키로 사용 (예: ScriptableObject 내부 설정)
                Managers.Instance.UIManager.Show<DashGameResultPopup>(0f, characterType).OnClickDialogue();
            }
            // 1분 30초 이상, 3분 30초 미만일 때 대사 출력
            else if (stopWatch.recodeTime < 210f)
            {
                // 2f는 내부에서 150f 기준 대사를 가져오는 키로 사용
                Managers.Instance.UIManager.Show<DashGameResultPopup>(1f, characterType).OnClickDialogue();
            }
            // 3분 30초 이상일 때 대사 출력
            else
            {
                // 3f는 내부에서 210f 기준 대사를 가져오는 키로 사용
                Managers.Instance.UIManager.Show<DashGameResultPopup>(2f, characterType).OnClickDialogue();
            }
        }
    }
}