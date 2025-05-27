using System;
using UnityEngine;

public class Chapter02Base : SceneBase
{
    protected override void InitSceneExtra(Action playIntroCallback)
    {
        Managers.Instance.DialogueManager.OnDialogStepEnd += HandleDialogStep;

        SkillForm();
        playIntroCallback?.Invoke();
    }

    // 씬이 로드되자마자 재생되는 컷신이 있다면 이 곳에 컷신이 끝났을 때 호출 될 콜백을 작성합니다.
    protected override void CutSceneEndCallback()
    {
        PlayChapterIntro(SquirrelTutorial);
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.InForest);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.ForestBird);
    }

    private void SkillForm()
    {
        Managers.Instance.GameManager.UnlockForm(PlayerFormType.Squirrel);
    }

    private void SquirrelTutorial()
    {
        Managers.Instance.AnalyticsManager.SendFunnel("11");
        var tutorial = Managers.Instance.UIManager.Show<UITutorial>();
        var skillPanel = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
        var squirrelBtn = skillPanel.squirrelBtn.GetComponent<RectTransform>();
        tutorial.SetTarget(squirrelBtn);
    }

    private void HandleDialogStep(int index)
    {
        if (index == 20002)
        {
            Managers.Instance.UIManager.Show<TutorialPopup>(2);
            Managers.Instance.DialogueManager.OnDialogStepEnd -= HandleDialogStep;
        }
    }
}