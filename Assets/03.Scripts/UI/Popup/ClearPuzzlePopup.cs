
public class ClearPuzzlePopup : PopupBase
{
    //TODO: 모든 퍼즐 시스템 통합 및 모든 퍼즐 팝업을 리펙토링하고 로직 수정
    private TreePuzzleSystem treePuzzle;
    private WirePuzzleSystem wirePuzzle;

    public override void Opened(params object[] param)
    {
        base.Opened(param);

        treePuzzle = null;
        wirePuzzle = null;

        foreach (var p in param)
        {
            if (p is TreePuzzleSystem tree)
                treePuzzle = tree;
            else if (p is WirePuzzleSystem wire)
                wirePuzzle = wire;
        }

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() =>
        {
            // 1) 팝업 닫기
            Managers.Instance.UIManager.Hide<ClearPuzzlePopup>();

            // 2) TreePuzzleSystem이 있으면 컷신 재생 & 진행도 업데이트
            if (treePuzzle != null)
                treePuzzle.OnClearButtonClicked();
            if (wirePuzzle != null)
                wirePuzzle.OnClearButtonClicked();
        });
    }
}
