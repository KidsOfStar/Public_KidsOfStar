using System;
using System.Collections;
using UnityEngine;

public class Chapter04Base : SceneBase
{
    [Header("Chapter 4")]
    [SerializeField] private int[] puzzleClearDialog;

    protected override void InitSceneExtra(Action callback)
    {
        callback?.Invoke();
        Managers.Instance.GameManager.OnProgressUpdated += AddListenerTutorial;
        Managers.Instance.DialogueManager.OnDialogStepStart += RecordPuzzleClear;
        SkillUnlock();
    }
    
    protected override void CutSceneEndCallback()
    {
        PlayChapterIntro();
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.City);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.City);
    }

    private void SkillUnlock()
    {
        Managers.Instance.GameManager.UnlockForm(PlayerFormType.Dog);
        Managers.Instance.GameManager.UnlockForm(PlayerFormType.Squirrel);
    }

    private void AddListenerTutorial()
    {
        if (!isFirstTime) return;

        if (Managers.Instance.GameManager.ChapterProgress == 2)
        {
            StartCoroutine(CatTutorial());
        }
    }

    private IEnumerator CatTutorial()
    {
        yield return new WaitForSeconds(1.5f);
        
        var tutorial = Managers.Instance.UIManager.Show<UITutorial>();
        var skillPanel = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
        var catBtn = skillPanel.catBtn.GetComponent<RectTransform>();
        tutorial.SetTarget(catBtn);
    }
    
    private void RecordPuzzleClear(int dialogIndex)
    {
        for (int i = 0; i < puzzleClearDialog.Length; i++)
        {
            if (puzzleClearDialog[i] != dialogIndex) continue;

            var analytics = Managers.Instance.AnalyticsManager;
            analytics.RecordChapterEvent("MapPuzzle",
                                         ("PuzzleNumber", i),
                                         ("FallCount", analytics.fallCount));

            analytics.fallCount = 0;
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Managers.Instance.GameManager.OnProgressUpdated -= AddListenerTutorial;
        Managers.Instance.DialogueManager.OnDialogStepStart -= RecordPuzzleClear;
    }
}
