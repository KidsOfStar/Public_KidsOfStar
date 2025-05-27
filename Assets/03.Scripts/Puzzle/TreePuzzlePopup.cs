using UnityEngine;
using UnityEngine.UI;

public class TreePuzzlePopup : PopupBase
{
    [SerializeField] private Button[] exitButtons;
    [SerializeField] private Button cancelButton;

    [SerializeField] private TreePuzzleSystem currentPuzzle;

    [SerializeField] private Image hintImage;

    private void Awake()
    {
        // 팝업 닫기버튼
        cancelButton.onClick.AddListener(() =>
        {
            Managers.Instance.SoundManager.PlaySfx(SfxSoundType.UICancel);
            OnCancelButtonClicked();
        });

        // 팝업 닫기 버튼
        foreach (var btn in exitButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                Managers.Instance.SoundManager.PlaySfx(SfxSoundType.UICancel);
                OnCancelButtonClicked();
            });
        }
    }

    public override void Opened(params object[] param)
    {
        EditorLog.Log("실행");
        base.Opened(param);

        var data = param[0] as TreePuzzleData;
        if (data == null)
        {
            EditorLog.LogError("TreePuzzlePopup: PuzzleData 누락!");
            return;
        }

        hintImage.sprite = data.backgroundSprite;

        int index = (int)param[1];

        Managers.Instance.GameManager.Player.Controller.IsControllable = false;
        currentPuzzle.SetupPuzzle(data, index);
        currentPuzzle.GeneratePuzzle();
        currentPuzzle.StartPuzzle();
    }

    private void OnCancelButtonClicked()
    {
        currentPuzzle.StopPuzzle();
        currentPuzzle.OnExit();
    }
}
