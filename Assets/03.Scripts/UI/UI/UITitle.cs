using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITitle : UIBase
{
    [Header("Button")]
    public Button startBtn;
    public Button exitBtn;
    public Button optionBtn;
    public Button loadBtn;

    private bool isGameStart = false; // 게임 시작 여부

    // Start is called before the first frame update
    void Start()
    {
        OnClickListener(startBtn, OnStartBtnClick, SfxSoundType.UIButton);
        OnClickListener(exitBtn, OnExitBtnClick, SfxSoundType.UIButton);
        OnClickListener(optionBtn, OnOptionBtnClick, SfxSoundType.UIButton);
        OnClickListener(loadBtn, OnLoadBtnClick, SfxSoundType.UIButton);
    }
    // 따로 스크립트로 빼서 관리하는게 좋을듯
    private void OnClickListener(Button button, UnityAction callback, SfxSoundType sfxType)
    {
        button.onClick.AddListener(() =>
        {
            Managers.Instance.SoundManager.PlaySfx(sfxType); // 효과음 재생
            callback?.Invoke(); // 콜백 실행
        });
    }

    private void OnLoadBtnClick()
    {
        // 로드 Popup 띄우기
        Managers.Instance.UIManager.Show<LoadPopup>();
        //Managers.Instance.UIManager.Show<SavePopup>();

    }

    private void OnOptionBtnClick()
    {
        // 옵션 Popup 띄우기
        Managers.Instance.UIManager.Show<OptionPopup>();
    }

    private void OnExitBtnClick()
    {
        Managers.Instance.UIManager.Show<WarningEndTop>(); // 종료 경고창 띄우기
    }

    private void OnStartBtnClick()
    {
        if (isGameStart)
        {
            return;
        }

        isGameStart = true; // 게임 시작
        Managers.Instance.AnalyticsManager.SendFunnel("2");

        // 게임 시작 시 로드 씬으로 이동
        Managers.Instance.GameManager.SetNewGame();
        LoadScene();
    }

    private void LoadScene()
    {
        Managers.Instance.SceneLoadManager.LoadScene(SceneType.Chapter1);
    }
}
