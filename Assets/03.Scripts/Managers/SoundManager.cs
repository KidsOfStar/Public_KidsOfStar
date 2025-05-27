using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class SoundManager : ISceneLifecycleHandler
{
    private readonly Transform sourceParent;
    private readonly AudioSource bgmSource;
    private readonly AudioSource ambienceSource;
    private readonly AudioSource sfxSource;
    private readonly AudioSource footstepSource;
    
    private const float AmbienceVolumeOffset = 0.2f;
    public Action<float> SetBgmVolumeCallback { get; set; }

    public SoundManager()
    {
        sourceParent = new GameObject("AudioSource").transform;
        sourceParent.SetParent(Managers.Instance.transform);
        GameObject audioSource =
            Managers.Instance.ResourceManager.Load<GameObject>($"{Define.prefabPath}{Define.audioSourceKey}");
        GameManager gameManager = Managers.Instance.GameManager;

        bgmSource = Object.Instantiate(audioSource, sourceParent).GetComponent<AudioSource>();
        bgmSource.name = "BGM";
        bgmSource.loop = true;
        bgmSource.volume = gameManager.BgmVolume;
        
        ambienceSource = Object.Instantiate(audioSource, sourceParent).GetComponent<AudioSource>();
        ambienceSource.name = "Ambience";
        ambienceSource.loop = true;
        ambienceSource.volume = Mathf.Max(0f, gameManager.BgmVolume - AmbienceVolumeOffset);

        sfxSource = Object.Instantiate(audioSource, sourceParent).GetComponent<AudioSource>();
        sfxSource.name = "SFX";
        sfxSource.loop = false;
        sfxSource.volume = gameManager.SfxVolume;

        footstepSource = Object.Instantiate(audioSource, sourceParent).GetComponent<AudioSource>();
        footstepSource.name = "Footstep";
        footstepSource.loop = false;
        footstepSource.volume = gameManager.SfxVolume;

        var audioListener = new GameObject("AudioListener");
        audioListener.AddComponent<AudioListener>();
        audioListener.transform.SetParent(sourceParent);
    }
    
    private void AttachAudioToCamera()
    {
        var currentScene = Managers.Instance.SceneLoadManager.CurrentScene;
        if (currentScene == SceneType.Title) return;

        var camera = Managers.Instance.GameManager.MainCamera;
#if UNITY_EDITOR
        if (Managers.Instance.IsDebugMode)
        {
            camera = Camera.main;
            sourceParent.SetParent(camera.transform);
            return;
        }
#endif
        if (camera == null)
        {
            EditorLog.LogError("SoundManager : Camera is not found.");
            return;
        }

        sourceParent.SetParent(camera.transform);
    }

    private void ReparentAudioToSoundManager()
    {
        var currentScene = Managers.Instance.SceneLoadManager.CurrentScene;
        if (currentScene == SceneType.Title)
            return;

        if (!sourceParent)
        {
            EditorLog.LogError("SoundManager : AudioSource is not found.");
            return;
        }

        sourceParent.SetParent(Managers.Instance.transform);
    }

    // BGM 재생(Loop)
    public void PlayBgm(BgmSoundType sound)
    {
        var resourceManager = Managers.Instance.ResourceManager;
        AudioClip audioClip = resourceManager.Load<AudioClip>($"{Define.bgmPath}{sound.GetName()}");

        if (!audioClip)
        {
            EditorLog.LogError($"SoundManager : {sound.GetName()} is not found.");
            return;
        }

        bgmSource.clip = audioClip;
        bgmSource.Play();
    }
    
    public void PlayAmbience(AmbienceSoundType sound)
    {
        var resourceManager = Managers.Instance.ResourceManager;
        AudioClip audioClip = resourceManager.Load<AudioClip>($"{Define.ambiencePath}{sound.GetName()}");

        if (!audioClip)
        {
            EditorLog.LogError($"SoundManager : {sound.GetName()} is not found.");
            return;
        }
        
        ambienceSource.clip = audioClip;
        ambienceSource.Play();
    }

    // BGM 정지
    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
        ambienceSource.clip = null;
    }

    // 효과음 재생
    public void PlaySfx(SfxSoundType sound)
    {
        var resourceManager = Managers.Instance.ResourceManager;
        AudioClip audioClip = resourceManager.Load<AudioClip>($"{Define.sfxPath}{sound.GetName()}");

        if (!audioClip)
        {
            EditorLog.LogError($"SoundManager : {sound.GetName()} is not found.");
            return;
        }
        
        sfxSource.PlayOneShot(audioClip);
    }

    // 발소리 재생
    public void PlayFootstep(FootstepType sound)
    {
        var resourceManager = Managers.Instance.ResourceManager;
        AudioClip audioClip = resourceManager.Load<AudioClip>($"{Define.sfxPath}{sound.GetName()}");

        if (!audioClip)
        {
            EditorLog.LogError($"SoundManager : {sound.GetName()} is not found.");
            return;
        }
        
        footstepSource.PlayOneShot(audioClip);
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
        ambienceSource.volume = Mathf.Max(0f, volume - AmbienceVolumeOffset);
        SetBgmVolumeCallback?.Invoke(volume);
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
        footstepSource.volume = volume;
    }

    public void OnSceneLoaded()
    {
        AttachAudioToCamera();
    }

    public void OnSceneUnloaded()
    {
        ReparentAudioToSoundManager();
        StopBgm();
        StopAmbience();
    }
}