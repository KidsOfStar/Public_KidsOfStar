using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WirePuzzleSystem : MonoBehaviour
{
    [Header("외부 참조")]
    [SerializeField, Tooltip("퍼즐 조각 프리팹")] private GameObject piecePrefab;
    [SerializeField, Tooltip("볼트 프리팹")] private GameObject boltPrefab;
    [SerializeField, Tooltip("퍼즐 조각이 배치될 패널")] private Transform puzzlePanel;
    [SerializeField, Tooltip("선택 영역의 RectTransform")] private RectTransform selectionBox;
    [SerializeField, Tooltip("배경 이미지")] private Image backgroundImage;

    [SerializeField, Tooltip("조각의 크기")] private float cellSize = 75f;
    [SerializeField, Tooltip("padding 보정용")] private Vector2 offset = new Vector2(15f, 15f);

    [Header("UI")]
    [SerializeField, Tooltip("타이머 텍스트")] private TextMeshProUGUI timerTxt;

    // 퍼즐 트리거 딕셔너리
    Dictionary<int, WirePuzzleTrigger> triggerMap;
    // 정답 조각들
    private List<Sprite> correctSprites;
    // 퍼즐 조각 배열
    private WirePuzzlePiece[,] puzzleGrid;
    // 퍼즐 데이터
    private WirePuzzleData puzzleData;
    // 선택 영역의 좌표
    private int selectX = 0;
    private int selectY = 0;
    // 퍼즐 한 줄의 조각 수
    private int gridWidth = 4;
    // 현재 퍼즐 ID
    private int puzzleIndex;
    // 제한 시간
    private float timeLimit;
    // 남은 시간
    [SerializeField, Tooltip("타임 오버 테스트를 위함")]private float currentTime;
    // 퍼즐 진행 여부
    private bool isRunning;
    public bool IsRunning => isRunning;

    // 퍼즐 시도 횟수
    private int challengeCount = 0;

    private void Awake()
    {
        // 씬 내 모든 트리거 탐색 및 연결
        triggerMap = new Dictionary<int, WirePuzzleTrigger>();

        foreach(var trigger in FindObjectsOfType<WirePuzzleTrigger>())
        {
            triggerMap[trigger.SequenceIndex] = trigger;
        }
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        timerTxt.text = Mathf.CeilToInt(currentTime).ToString();

        if(currentTime <= 0f)
        {
            FailPuzzle();
        }
    }

    // 퍼즐 데이터 초기화
    public void SetupPuzzle(WirePuzzleData data, int idx)
    {
        puzzleData = data;
        puzzleIndex = idx;

        correctSprites = new List<Sprite>(data.pieceSprites);
        gridWidth = data.gridWidth;

        bool isEasy = Managers.Instance.GameManager.Difficulty == Difficulty.Easy;
        timeLimit = isEasy ? data.easyTimeLimit : data.hardTimeLimit;

        if (backgroundImage != null)
            backgroundImage.sprite = data.backgroundSprite;
    }

    public void GeneratePuzzle()
    {
        foreach(Transform child in puzzlePanel)
        {
            if(child.TryGetComponent<WirePuzzlePiece>(out _)
                || child.TryGetComponent<BoltButtonHandler>(out _))
            {
                Destroy(child.gameObject);
            }
        }

        puzzleGrid = new WirePuzzlePiece[gridWidth, gridWidth];

        // 퍼즐 조각 생성&배열에 배치
        for(int i = 0; i < gridWidth; i++)
        {
            for(int j = 0; j < gridWidth; j++)
            {
                // 퍼즐 조각 생성
                GameObject go = Instantiate(piecePrefab, puzzlePanel);
                WirePuzzlePiece piece = go.GetComponent<WirePuzzlePiece>();

                // 테스트용 스프라이트 적용
                Sprite spriet = correctSprites[i * gridWidth + j];
                piece.InitPiece(j, i, spriet);
                piece.WireColor = GetColorType(j);
                puzzleGrid[j, i] = piece;
            }
        }
        SpawnBolt();
    }

    private WireColorType GetColorType(int x)
    {
        return x switch
        {
            0 => WireColorType.Yellow,
            1 => WireColorType.Blue,
            2 => WireColorType.Red,
            3 => WireColorType.Green,
            _ => WireColorType.Green
        };
    }

    // 퍼즐 시작
    public void StartPuzzle()
    {
        currentTime = timeLimit;
        isRunning = true;

        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.CityPuzzle);
        challengeCount++;
    }

    // 퍼즐 중단
    public void StopPuzzle()
    {
        isRunning = false;

        if (triggerMap.TryGetValue(puzzleIndex, out var trigger))
        {
            trigger.ResetTrigger();
        }
    }

    // 퍼즐 실패
    private void FailPuzzle()
    {
        isRunning = false;
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.PuzzleFail);
        OnExit();
        Managers.Instance.UIManager.Show<GameOverPopup>();

        if(triggerMap.TryGetValue(puzzleIndex, out var trigger))
        {
            trigger.ResetTrigger();
        }
    }

    // 퍼즐 클리어
    private void CompletePuzzle()
    {
        isRunning = false;

        float clearTime = Mathf.CeilToInt(timeLimit - currentTime);
        Managers.Instance.AnalyticsManager.RecordChapterEvent("PopUpPuzzle",
            ("PuzzleNumber", puzzleIndex), 
            ("ChallengeCount", challengeCount),
            ("ClearTime", clearTime));
        challengeCount = 0;
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.PuzzleClear);
        Managers.Instance.UIManager.Show<ClearPuzzlePopup>(this);
        OnExit();

        var sequence = puzzleIndex == 0 ? 32
                     : puzzleIndex == 1 ? 33
                     : puzzleIndex == 2 ? 34
                     : puzzleIndex == 3 ? 38
                     : puzzleIndex == 4 ? 39
                     : puzzleIndex == 5 ? 40
                     : -1;

        if (sequence > 0)
            Managers.Instance.AnalyticsManager.SendFunnel(sequence.ToString());
    }

    // 클리어 팝업 내 버튼 클릭 시 처리
    public void OnClearButtonClicked()
    {
        if(triggerMap.TryGetValue(puzzleIndex, out var trigger))
        {
            trigger.DisableExclamation();
            // 엘리베이터 잠금 해제
            trigger.LockedElevator.UnlockElevator();
        }
    }

    // 팝업 닫고 제어 복구
    public void OnExit()
    {
        Managers.Instance.UIManager.Hide<WirePuzzlePopup>();
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.City);
        Managers.Instance.GameManager.Player.Controller.UnlockPlayer();
    }

    // 나사 생성
    public void SpawnBolt()
    {
        // 퍼즐 전체 너비
        float totalWidth = gridWidth * cellSize;
        // 퍼즐 전체 높이
        float totalHeight = gridWidth * cellSize;
        // 퍼즐 중심이 (0,0)이 되도록 하기 위한 오프셋
        Vector2 centerOffset = new Vector2(totalWidth / 2f, totalHeight / 2f);

        for (int i = 0; i < gridWidth - 1; i++)
        {
            for (int j = 0; j < gridWidth - 1; j++)
            {
                // 나사 프리팹 생성
                GameObject bolt = Instantiate(boltPrefab, puzzlePanel);
                RectTransform rt = bolt.GetComponent<RectTransform>();

                // 모서리 위치
                float x = (j + 1) * cellSize - centerOffset.x;
                float y = -((i + 1) * cellSize - centerOffset.y);

                rt.anchoredPosition = new Vector2(x, y);

                int capturedX = j;
                int capturedY = i;

                bolt.GetComponent<BoltButtonHandler>().Init(capturedX, capturedY);
                bolt.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (MoveSelection(capturedX, capturedY))
                    {
                        bolt.transform.Rotate(Vector3.forward, 90f);
                    }
                });
            }
        }
        
        ShufflePuzzle();
        selectX = 0;
        selectY = 0;
        UpdateSelectionBoxPosition();
        selectionBox.SetAsLastSibling();
    }

    #region 나무껍질 퍼즐에는 없는 함수들
    // 생성된 퍼즐 조각 섞기
    private void ShufflePuzzle()
    {
        do
        {
            for (int i = 0; i < puzzleData.ShuffleCount; i++)
            {
                selectX = UnityEngine.Random.Range(0, gridWidth - 1);
                selectY = UnityEngine.Random.Range(0, gridWidth - 1);
                int rotateCount = UnityEngine.Random.Range(1, 3);

                for (int j = 0; j < rotateCount; j++)
                {
                    RotateSelection();
                }
            }
        } while(CheckPuzzleClear());
    }

    // 선택 영역의 조각 회전
    public bool MoveSelection(int dx, int dy)
    {
        // 이미 선택된 위치의 나사를 다시 클릭
        if(dx == selectX && dy == selectY)
        {
            RotateSelection();
            return true;
        }
        else
        {
            // 선택 영역 위치 갱신
            selectX = Mathf.Clamp(dx, 0, gridWidth - 2);
            selectY =Mathf.Clamp(dy, 0, gridWidth - 2);
            UpdateSelectionBoxPosition();
            return false;
        }
    }

    // 선택 영역 좌표 수정
    private void UpdateSelectionBoxPosition()
    {
        // 퍼즐의 전체 크기
        float totalWidth = gridWidth * cellSize;
        float totalHeight = gridWidth * cellSize;
        Vector2 centerOffset = new Vector2(totalWidth / 2f, totalHeight / 2f);

        // 선택된 나사 기준으로 선택영역 위치 설정
        float x = (selectX + 1) * cellSize - centerOffset.x;
        float y = -((selectY + 1) * cellSize - centerOffset.y);

        selectionBox.anchoredPosition = new Vector2(x, y);
    }

    // 선택 영역 내의 조각 스프라이트 시계방향으로 교체
    public void RotateSelection()
    {
        int x = selectX;
        int y = selectY;

        // 선택 영역 내의 조각 참조
        WirePuzzlePiece p1 = puzzleGrid[x, y];
        WirePuzzlePiece p2 = puzzleGrid[x + 1, y];
        WirePuzzlePiece p3 = puzzleGrid[x + 1, y + 1];
        WirePuzzlePiece p4 = puzzleGrid[x, y + 1];

        // 스프라이트 참조
        Sprite s1 = p1.GetSprite();
        Sprite s2 = p2.GetSprite();
        Sprite s3 = p3.GetSprite();
        Sprite s4 = p4.GetSprite();

        // 배선 색상 참조
        WireColorType c1 = p1.WireColor;
        WireColorType c2 = p2.WireColor;
        WireColorType c3 = p3.WireColor;
        WireColorType c4 = p4.WireColor;

        // 회전 적용
        p1.SetSprite(s4);
        p2.SetSprite(s1);
        p3.SetSprite(s2);
        p4.SetSprite(s3);

        p1.WireColor = c4;
        p2.WireColor = c1;
        p3.WireColor = c2;
        p4.WireColor = c3;

        if (CheckPuzzleClear())
        {
            CompletePuzzle();
        }
    }

    // 퍼즐 클리어 체크
    private bool CheckPuzzleClear()
    {
        for(int x = 0; x < gridWidth; x++)
        {
            WireColorType wColor = GetColorType(x);
            for(int y = 0; y < gridWidth; y++)
            {
                if (puzzleGrid[x, y].WireColor != wColor)
                    return false;
            }
        }

        return true;
    }
    #endregion
}