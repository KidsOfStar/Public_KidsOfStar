using UnityEngine;
using UnityEngine.Timeline;

// 컷씬용 BGM 플레이어
public class BgmPlayer : MonoBehaviour
{
    [field: SerializeField] public BgmSoundType BgmSoundType {get; private set;}
    [field: SerializeField] public AmbienceSoundType AmbienceSoundType {get; private set;}
    [field: SerializeField] public SignalAsset PlayBgmSignal {get; private set;}

    public void PlayBgm()
    {
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType);
    }

    // TODO: 컷씬에서 BGM을 멈춰야 하는 경우가 생긴다면 사용
    // TODO: StopBgmSignal을 생성하고, 연결해주기
    public void StopBGM()
    {
        Managers.Instance.SoundManager.StopBgm();
        Managers.Instance.SoundManager.StopAmbience();
    }
}