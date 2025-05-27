using System;
using UnityEngine;

public class Chapter03Base : SceneBase
{
    [SerializeField] private DashGame dashGame;
    [SerializeField] private DashInteractable[] dashInteractable;
    
    protected override void InitSceneExtra(Action playIntroCallback)
    {
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.WithDogs);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.Wind);
        if (isFirstTime)
        {
            Managers.Instance.CutSceneManager.PlayCutScene(CutSceneType.DogFormChange, playIntroCallback);
            Managers.Instance.AnalyticsManager.SendFunnel("18");
        }

        DashGameInit();
        SkillForm();
    }

    // 씬이 로드되자마자 재생되는 컷신이 있다면 이 곳에 컷신이 끝났을 때 호출 될 콜백을 작성합니다.
    protected override void CutSceneEndCallback()
    {
        PlayChapterIntro(DogTutorial);
    }

    private void DashGameInit()
    {
        dashGame.Setting();

        for(int i = 0; i < dashInteractable.Length; i++)
        {
            dashInteractable[i].Init(dashGame);
        }
    }

    private void SkillForm()
    {
        // GameManager에 'Dog' 스킬 해금 기록
        // 이벤트 발생 시킴
        Managers.Instance.GameManager.UnlockForm(PlayerFormType.Dog);
    }

    private void DogTutorial()
    {
        var tutorial = Managers.Instance.UIManager.Show<UITutorial>();
        var skillPanel = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
        var dogBtn = skillPanel.dogBtn.GetComponent<RectTransform>();
        tutorial.SetTarget(dogBtn);
    }
}