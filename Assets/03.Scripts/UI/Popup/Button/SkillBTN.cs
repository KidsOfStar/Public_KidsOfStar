using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillBTN : UIBase
{
    [Header("UI SkillBtn")] // 스킬 버튼들
    public Button jumpBtn;
    public Button hideBtn;
    public Button catBtn;
    public Button dogBtn;
    public Button squirrelBtn;
    public Button interactionBtn;

    [Header("UI HideSkill")] // 스킬 UI 오브젝트들 (버튼 아래 표시용)
    public GameObject hideSkill;
    public GameObject catSkill;
    public GameObject dogSkill;
    public GameObject squirrelSkill;

    public Action OnInteractBtnClick { get; set; }

    public SkillUnlock skillUnlock; // 스킬 잠금 해제 UI
    private bool isSkillActive = false; // 스킬 UI 활성화 여부
    private float skillCooldown = 0.5f; // 스킬 쿨타임
    public bool isSkillAdd = false; // 스킬 활성화 여부

    // Start is called before the first frame update
    void Start()
    {
        jumpBtn.onClick.AddListener(OnJump);
        hideBtn.onClick.AddListener(OnHide);
        catBtn.onClick.AddListener(OnCat);
        dogBtn.onClick.AddListener(OnDog);
        squirrelBtn.onClick.AddListener(OnSquirrel);

        OnClickListener(interactionBtn, OnInteraction, SfxSoundType.Communication); // 상호작용 버튼 클릭 시 효과음 재생

        interactionBtn.gameObject.SetActive(false); // 상호작용 버튼 비활성화

        skillUnlock = GetComponent<SkillUnlock>();
    }

    private void Update()
    {
        if (Managers.Instance.GameManager.Player != null)
        {
            IsGruoud(); // 플레이어가 땅에 있는지 확인
        }
    }

    private void OnClickListener(Button button, UnityAction callback, SfxSoundType sfxType)
    {
        button.onClick.AddListener(() =>
        {
            Managers.Instance.SoundManager.PlaySfx(sfxType); // 효과음 재생
            callback?.Invoke(); // 콜백 실행
        });
    }

    // 플레이어가 땅에 있는지 확인하여 스킬 UI 표시 여부 결정
    private void IsGruoud()
    {
        //플레이어가 땅에 있는지 확인하여 스킬 UI 표시 여부 결정
        if (Managers.Instance.GameManager.Player.Controller.IsGround)
        {
            ToggleSkillUI(true);  // 땅에 닿았으면 스킬 UI 활성화
        }
        else
        {
            ToggleSkillUI(false); // 공중에 있으면 스킬 UI 비활성화
        }
    }

    public void OnJump()
    {
        //ToggleSkillUI(false); // 점프 시 스킬 UI 숨기기
        OnBlink(skillUnlock.jumpBG, 0.1f); // 배경 깜빡임 효과

        // 공중일 경우 점프 불가
        //if (!Managers.Instance.GameManager.Player.Controller.IsGround)
        //    return;

        // 실제 점프 실행
        Managers.Instance.GameManager.Player.Controller.Jump();
    }

    //public void OnHide() => UseSkill("Hide", skillUnlock.hideIcon, skillUnlock.hideBG);
    public void OnHide()
    {
        // 태그 Hide Area에 있는지 확인
        if (Managers.Instance.GameManager.Player.FormControl.isInHideArea)
            UseSkill(PlayerFormType.Hide, skillUnlock.hideIcon, skillUnlock.hideBG);
    }


    public void OnCat() => UseSkill(PlayerFormType.Cat, skillUnlock.catIcon, skillUnlock.catBG);

    /// <summary>
    /// 개 스킬 버튼 클릭 시 호출
    /// </summary>
    /*    public void OnDog()
    {
        UseSkill("Dog", skillUnlock.dogIcon, skillUnlock.dogBG);
    }*/
    public void OnDog() => UseSkill(PlayerFormType.Dog, skillUnlock.dogIcon, skillUnlock.dogBG);
    public void OnSquirrel() => UseSkill(PlayerFormType.Squirrel, skillUnlock.squirrelIcon, skillUnlock.squirrelBG);
    public void UseSkill(PlayerFormType formType, GameObject icon, GameObject bg)
    {
        // 스킬이 이미 활성화된 상태라면 중복 실행을 방지하고 메서드 종료
        if (isSkillActive) return;

        // 스킬 아이콘이 없을 경우, 스킬 비활성화
        // skillUnlock.SquirrelIcon이 현재 활성화된 상태와 isSkillAdd 상태가 같다면 스킬을 사용할 수 없음
        if (icon.activeSelf == isSkillAdd)
        {
            // 스킬 아이콘을 비활성화하고 스킬 실행을 중단 (해금되지 않음)
            icon.SetActive(false);
            return;
        }

        icon.SetActive(true);

        // 스킬 활성화 상태로 변경
        isSkillActive = true;

        Managers.Instance.GameManager.Player.FormControl.FormChange(formType);

        if (bg.activeSelf)
        {
            bg.SetActive(false);
        }
        else
        {
            skillUnlock.ShowSkillBG(bg);
        }
        StartCoroutine(ResetSkillCooldown(skillCooldown));
    }

    // 상호작용 버튼 클릭 시 대화 시작
    public void OnInteraction()
    {
        OnInteractBtnClick?.Invoke();
        Managers.Instance.DialogueManager.OnClick?.Invoke();
    }

    public void ShowInteractionButton(bool isActive)
    {
        interactionBtn.gameObject.SetActive(isActive);
    }

    // UI 깜빡임 효과 코루틴
    private IEnumerator BlinkEffect(GameObject obj, float duration)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }

    // 깜빡임 효과 실행 함수
    public void OnBlink(GameObject obj, float duration)
    {
        StartCoroutine(BlinkEffect(obj, duration));
    }

    private void ToggleSkillUI(bool isActive)
    {
        hideSkill.SetActive(isActive);
        catSkill.SetActive(isActive);
        dogSkill.SetActive(isActive);
        squirrelSkill.SetActive(isActive);
    }

    private IEnumerator ResetSkillCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        isSkillActive = false;
    }
}
