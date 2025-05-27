using System;
using UnityEngine;
using System.Net.NetworkInformation;

public class Chapter0502Base : SceneBase
{
    [SerializeField] private SafePuzzleTrigger safePuzzleTrigger;
    protected override void CutSceneEndCallback() { }

    protected override void InitSceneExtra(Action callback)
    {
        safePuzzleTrigger.init();
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.Aquarium);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.Aquarium);

        if (isFirstTime)
            Managers.Instance.GameManager.UpdateProgress(); // 진행도 2
        else
            Managers.Instance.GameManager.SetLoadedProgress();
    }
}