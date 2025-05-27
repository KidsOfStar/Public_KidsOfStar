using System;
using UnityEngine;

// 챕터 1 특이사항
// 챕터 1에서 재생해야하는 컷씬이 두개라서 좀 복잡해졌습니다.
// 챕터 1 진행도는 총 4단계이며, 각 단계는 다음과 같습니다.
// 1. 마오름과 첫 대화(대화로 인해 진행도 1 증가 및 인간 폼으로 변경)
// 2. 마오름과 한번 더 대화 후 특정 장소에 도달하면 진행도 1 증가 : 이 때 NPC들의 위치 변경
// 3. 마지막으로 마오름과 대화하면 진행도 1 증가 및 추격 씬으로 이동
// 4. 추격 씬을 클리어하면 챕터 1 종료 및 씬 이동하여 챕터 2로 이동
public class Chapter01Base : SceneBase
{
    [Header("Chapter 1")]
    [SerializeField] private SceneEventTrigger sceneEventTrigger;

    private bool isTutorial = true;
    
    protected override void InitSceneExtra(Action callback)
    {
        Managers.Instance.AnalyticsManager.SendFunnel("3");
        Managers.Instance.CutSceneManager.PlayCutScene(CutSceneType.FallingDown, callback);
        sceneEventTrigger.Init();
    }

    // 씬이 로드되자마자 재생되는 컷신이 있다면 이 곳에 컷신이 끝났을 때 호출 될 콜백을 작성합니다.
    protected override void CutSceneEndCallback()
    {
        Managers.Instance.AnalyticsManager.SendFunnel("4");
        PlayChapterIntro(ChapterIntroEndCallback);
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.Maorum);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.UnderWater);
    }

    private void ChapterIntroEndCallback()
    {
        Managers.Instance.AnalyticsManager.SendFunnel("5");
        var tutorial = Managers.Instance.UIManager.Show<UITutorial>();
        var joystick = Managers.Instance.UIManager.Get<UIJoystick>();
        tutorial.SetTarget(joystick.joystickBase);
    }
    
    public void InteractTutorial()
    {
        if (!isTutorial) return;
        isTutorial = false;
        
        Managers.Instance.AnalyticsManager.SendFunnel("6");
        var tutorial = Managers.Instance.UIManager.Show<UITutorial>();
        var skillPanel = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
        var interactBtn = skillPanel.interactionBtn.GetComponent<RectTransform>();

        tutorial.SetTarget(interactBtn);
    }
}