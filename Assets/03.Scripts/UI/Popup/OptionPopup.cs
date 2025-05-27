using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : PopupBase
{
    [Header("UI Audio")]
    public Slider bgmSlider; // BGM 슬라이더
    public Slider sfxSlider; // SFX 슬라이더

    [Header("UI Btn")]
    //public Button retryBtn; // 재시작 버튼
    public Button endBtn; // 메인 메뉴 버튼

    private SoundManager soundManager;

    protected override void Start()
    {
        base.Start();
        soundManager = Managers.Instance.SoundManager;

        InitSlider();
        ButtonClick();
        // Title Scene에서 다시하기 및 게임 종료 안 나오게
        if (Managers.Instance.SceneLoadManager.CurrentScene == SceneType.Title)
        {
            //retryBtn.gameObject.SetActive(false);
            endBtn.gameObject.SetActive(false);
        }
    }

    private void InitSlider()
    {

        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 0.7f); // 기본값 0.7
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.7f); // 기본값 0.7
        EditorLog.Log($"BGM Volume : {bgmSlider.value}");
        // 슬라이더 초기화
        bgmSlider.onValueChanged.AddListener(soundManager.SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(soundManager.SetSfxVolume);
    }


    public void ButtonClick()
    {
        // 버튼 클릭 이벤트 등록
        //retryBtn.onClick.AddListener(OnClickRetryBtn);
        endBtn.onClick.AddListener(OnExitBtnClick);
    }

    //public void OnClickRetryBtn()
    //{
    //    // 현재 씬에서 재시작
    //    string currentSceneName = SceneManager.GetActiveScene().name;
    //    SceneManager.LoadScene(currentSceneName);
    //}

    private void OnExitBtnClick()
    {
        Managers.Instance.UIManager.Show<WarningEndTop>(); // 종료 경고창 띄우기
        HideDirect();
    }
    public override void HideDirect()
    {
        base.HideDirect();
        Managers.Instance.GameManager.SaveVolumeSetting(bgmSlider.value, sfxSlider.value);
    }
}
