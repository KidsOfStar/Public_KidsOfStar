using UnityEngine;
using UnityEngine.UI;

// 플레이어 관련 UI를 제어하는 클래스
public class PlayerBtn : UIBase
{
    [Header("UI FunctionBtn")] // 게임 기능 버튼들
    public Button stopBtn;
    public Button skipBtn;

    [Header("UI Panel")] // 패널
    public SkillBTN skillPanel; // 스킬 패널
    public GameObject functionPanel; // 기능 패널
    public UIJoystick joystick;

    public void Init()
    {
        joystick = transform.parent.GetComponentInChildren<UIJoystick>();

        stopBtn.onClick.AddListener(OnOptionBtnClick);
        skipBtn.onClick.AddListener(OnSkip);

        Managers.Instance.CutSceneManager.OnCutSceneStart += OnCutSceneSkip;
        Managers.Instance.CutSceneManager.OnCutSceneEnd += OffCutSceneSkip;
    }

    // 정지 버튼 클릭 시 게임 일시정지
    private void OnOptionBtnClick()
    {
        Managers.Instance.UIManager.Show<OptionPopup>();
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.UIButton);
        Time.timeScale = 0;
        Managers.Instance.GameManager.Player.Controller.LockPlayer();
    }

    // 스킵 버튼 클릭 시 호출될 메소드 (현재 비어 있음)
    public void OnSkip()
    {
        // 씬 스킵 처리
    }

    public void OnCutSceneSkip()
    {
        skillPanel.SetActive(false); // 스킬 패널 비활성화
        joystick.SetActive(false); // 조이스틱 비활성화
        functionPanel.SetActive(false); // 기능 패널 비활성화
        stopBtn.gameObject.SetActive(false); // 정지 버튼 비활성화
        //skipBtn.gameObject.SetActive(true); // 스킵 버튼 활성화
        skipBtn.gameObject.SetActive(false); // 기능 구현 안 되어서 비활성화
    }
    
    public void OffCutSceneSkip()
    {
        skillPanel.SetActive(true); // 스킬 패널 비활성화
        joystick.SetActive(true); // 조이스틱 비활성화
        functionPanel.SetActive(true); // 기능 패널 비활성화
        stopBtn.gameObject.SetActive(true); // 정지 버튼 비활성화
        //skipBtn.gameObject.SetActive(false); // 스킵 버튼 활성화
        skipBtn.gameObject.SetActive(false); // 기능 구현 안 되어서 비활성화
    }

    private void OnDestroy()
    {
        Managers.Instance.CutSceneManager.OnCutSceneStart -= OnCutSceneSkip;
        Managers.Instance.CutSceneManager.OnCutSceneEnd -= OffCutSceneSkip;
    }
}
