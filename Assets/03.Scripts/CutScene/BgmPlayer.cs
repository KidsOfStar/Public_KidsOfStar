using UnityEngine;
using UnityEngine.Timeline;

// 컷씬용 BGM 플레이어
public class BgmPlayer : MonoBehaviour
{
    [field: SerializeField] public BgmSoundType BgmSoundType {get; private set;}
    [field: SerializeField] public AmbienceSoundType AmbienceSoundType {get; private set;}
    [field: SerializeField] public bool PlayAmbience {get; private set;} = true;
    [field: SerializeField] public SignalAsset PlayBgmSignal {get; private set;}
    [field: SerializeField] public SignalAsset StopBgmSignal {get; private set;}

    public void PlayBgm()
    {
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType);
        
        if (!PlayAmbience) return;
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType);
    }

    public void StopBGM()
    {
        Managers.Instance.SoundManager.StopBgm();
        Managers.Instance.SoundManager.StopAmbience();
    }
}