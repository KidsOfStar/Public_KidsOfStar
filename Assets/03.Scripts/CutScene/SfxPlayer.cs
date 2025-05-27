using UnityEngine;
using UnityEngine.Timeline;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private SignalAsset sfxSignal;
    [SerializeField] private SfxSoundType[] sfxSounds;
    private int currentIndex;
    
    public void PlaySfx()
    {
        if (sfxSounds.Length == 0)
        {
            EditorLog.LogError("SfxPlayer: sfxSounds is empty");
            return;
        }
        
        if (currentIndex >= sfxSounds.Length)
        {
            EditorLog.LogWarning("SfxPlayer: All SFX have been played");
            return;
        }

        Managers.Instance.SoundManager.PlaySfx(sfxSounds[currentIndex]);
        currentIndex++;
    }
}
