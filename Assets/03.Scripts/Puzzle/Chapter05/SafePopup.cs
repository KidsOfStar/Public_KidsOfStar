using UnityEngine;

public class SafePopup : PopupBase
{
    [SerializeField] private GameObject exclamationInstance;

    [Header("퍼즐 시스템")]
    private SceneType sceneType;
    public TreePuzzleData[] datas; // 전체 9개 퍼즐 SO
    public SafePuzzleSystem puzzleSystem; // 3개 퍼즐 시스템 (각 퍼즐 UI와 연결됨)
    public SafePuzzle safePuzzle; // 금고 퍼즐

    private int[] puzzleIndexs;        // 현재 씬에서 사용할 퍼즐 3개의 인덱스
    public int countIndex = 0;

    public int challengeCount;   // 도전 횟수


    public override void Opened(params object[] param)
    {
        if (param.Length > 0 && param[0] is SceneType scene)
        {
            sceneType = scene;
        }
        Managers.Instance.GameManager.Player.Controller.LockPlayer();

        puzzleIndexs = GetIndexSetForScene(sceneType); // 꼭 세팅해줘야 함

        StartPuzzleAtIndex(countIndex);
    }
    public void nextPuzzle()
    {
        countIndex++;

        if (countIndex >= puzzleIndexs.Length)
        {
            EditorLog.LogWarning("[SafePopup] 퍼즐이 모두 완료되었습니다. 더 이상 진행할 퍼즐이 없습니다.");
            return;
        }


        if (countIndex >= puzzleIndexs.Length)
        {
            EditorLog.LogError($"[SafePopup] curSceneIndex 배열 인덱스 초과! countIndex: {countIndex}, curSceneIndex.Length: {puzzleIndexs.Length}");
            return;
        }

        StartPuzzleAtIndex(countIndex);
    }


    // 실패 시 퍼즐 리셋
    public void FullReset()
    {
        countIndex = 0;

        puzzleSystem.ResetSystem();

        // 씬에 맞는 퍼즐 데이터 재설정
        puzzleIndexs = GetIndexSetForScene(sceneType);
    }

    public override void HideDirect()
    {
        base.HideDirect();
        Managers.Instance.GameManager.Player.Controller.UnlockPlayer();
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.Aquarium);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.Aquarium);
    }

    private void StartPuzzleAtIndex(int index)
    {
        challengeCount++;
        if (index >= puzzleIndexs.Length)
        {
            return;
        }

        var data = datas[puzzleIndexs[index]];

        puzzleSystem.SetupPuzzle(data, 2);
        puzzleSystem.GeneratePuzzle();
        puzzleSystem.StartPuzzle();
    }

    private int[] GetIndexSetForScene(SceneType sceneName)
    {
        switch (sceneName)
        {
            case SceneType.Chapter501:
                return new int[] { 0, 1, 2 }; // Chapter501에서 사용할 퍼즐 인덱스
            case SceneType.Chapter502:
                return new int[] { 3, 4, 5 }; // Chapter502에서 사용할 퍼즐 인덱스
            case SceneType.Chapter504:
                return new int[] { 6, 7, 8 }; // Chapter504에서 사용할 퍼즐 인덱스
            default:
                return new int[] { 0, 1, 2};
        }
    }
}
