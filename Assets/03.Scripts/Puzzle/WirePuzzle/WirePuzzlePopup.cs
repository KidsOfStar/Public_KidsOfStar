using UnityEngine;
using UnityEngine.UI;

public class WirePuzzlePopup : PopupBase
{
    [SerializeField, Tooltip("퍼즐 시스템 스크립트")] private WirePuzzleSystem currentPuzzle;
    [SerializeField, Tooltip("퍼즐 힌트 이미지")] private Image hintImage;

    private void Awake()
    {
        // 취소 버튼 클릭 시 퍼즐 종료
        closeBtn.onClick.AddListener(() =>
        {
            Managers.Instance.SoundManager.PlaySfx(SfxSoundType.UICancel);
            OnCancelButtonClicked();
        });
    }

    // 팝업이 열릴 때 호출
    public override void Opened(params object[] param)
    {
        base.Opened(param);

        // 첫 번째 인자는 퍼즐 데이터
        var data = param[0] as WirePuzzleData;
        if(data == null)
        {
            EditorLog.LogError("WirePuzzlePopup: PuzzleData 누락!");
            return;
        }
        
        // 배경 힌트 이미지 적용
        hintImage.sprite = data.backgroundSprite;

        // 두 번째 인자는 퍼즐 인덱스(몇 번째 퍼즐인지
        int index = (int)param[1];

        // 플레이어 조작 잠금
        Managers.Instance.GameManager.Player.Controller.LockPlayer();

        // 퍼즐 구성 & 실행
        currentPuzzle.SetupPuzzle(data, index);
        currentPuzzle.GeneratePuzzle();
        currentPuzzle.StartPuzzle();
    }

    // 퍼즐 취소 버튼 클릭 시 동작
    private void OnCancelButtonClicked()
    {
        // 퍼즐 정지
        currentPuzzle.StopPuzzle();
        currentPuzzle.OnExit();
    }
}
