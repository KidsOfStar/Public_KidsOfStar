using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmLayeredFader : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource[] mainSources;
    [SerializeField] private AudioSource[] loopEffects;

    [Header("Loop Time")]
    [SerializeField] private float mainLoopTime = 192.5f;
    [SerializeField] private float bufferOffset = 0.5f;
    
    [Header("Dialog")]
    [SerializeField] private int strMelodyIndex = 5012;
    [SerializeField] private int[] dialogIndexes;
    
    // 5022, 5023, 5024, 5026, 5027 시작할 때
    // [SerializeField] private int[] finalDialogs;

    private readonly Dictionary<MainBgmSourceType, AudioSource> audioDict = new();
    private readonly Dictionary<int, MainBgmSourceType> audioByIndexDict = new();
    
    // Audio Setting
    private int progress = 0;
    private bool isLoop = true;
    private double dspStart;
    
    // volume
    private float bgmVolume;
    private float loopEffectVolume;
    
    public void Init()
    {
        bgmVolume = Managers.Instance.GameManager.BgmVolume;
        SetVolume(bgmVolume);
        
        // BGM Stop
        var soundManager = Managers.Instance.SoundManager;
        soundManager.StopBgm();
        soundManager.StopAmbience();
        
        // 볼륨 변경 이벤트에 구독
        soundManager.SetBgmVolumeCallback += SetVolume;

        // 초기화
        InitDictionary();
        AddListener();
        
        // 음악 재생
        ScheduleLoop();
        StartCoroutine(LoopScheduler());

        var piano = audioDict[MainBgmSourceType.Piano];
        StartCoroutine(FadeInAudio(piano, 2f));
        progress++;
    }

    private void AddListener()
    {
        var dialogManager = Managers.Instance.DialogueManager;
        dialogManager.OnDialogStepEnd += OnPlayStrMelody;
        dialogManager.OnDialogStepStart += OnPlayAudioSources;

        var selectionPanel = Managers.Instance.UIManager.Show<UISelectionPanel>();
        selectionPanel.HideDirect();
        selectionPanel.OnFinalSelect += OnPlayRiseEffect;
    }

    private void ScheduleLoop()
    {
        EditorLog.Log("Loop Start");
        
        dspStart = AudioSettings.dspTime + bufferOffset;
        for (int i = 0; i < mainSources.Length - 1; i++)
        {
            var source = mainSources[i];
            source.PlayScheduled(dspStart);
        }
    }

    private void PlayLoopEffects()
    {
        if (progress > 7)
            loopEffects[1].PlayScheduled(dspStart);
            
        loopEffects[0].PlayScheduled(dspStart);
    }
    
    private IEnumerator LoopScheduler()
    {
        // stopLoop가 켜지기 전까지 매 loopTime마다 ScheduleLoop 호출
        while (isLoop)
        {
            double nextStart = dspStart + mainLoopTime;
            double earlyStart   = nextStart - 3.5;     // 3분 9초 시점에 loopEffect 재생
            double waitEarly    = earlyStart - bufferOffset;
            double waitSchedule = nextStart  - bufferOffset;
            
            // 조기 콜백(189초)까지 대기
            while (AudioSettings.dspTime < waitEarly)
            {
                if (!isLoop) yield break;
                yield return null;
            }
            PlayLoopEffects();  

            // 본 콜백(192초)까지 대기
            while (AudioSettings.dspTime < waitSchedule)
            {
                if (!isLoop) yield break;
                yield return null;
            }
            ScheduleLoop();
        }
    }

    // 5012번 대사가 끝나면 StrMelody 재생
    private void OnPlayStrMelody(int dialogIndex)
    {
        if (dialogIndex == strMelodyIndex)
        {
            PlaySource(MainBgmSourceType.StrMelody1);
            progress++;
        }
    }

    // 특정 대사가 시작될 때 3~8번 트랙 재생
    private void OnPlayAudioSources(int dialogIndex)
    {
        for (int i = 0; i < dialogIndexes.Length; i++)
        {
            if (dialogIndex != dialogIndexes[i]) continue;

            var audioType = audioByIndexDict[dialogIndexes[i]];
            PlaySource(audioType);
            progress++;
        }
    }

    // 9번 라이즈 이펙트 재생과 동시에 2~8번 멈춤
    private void OnPlayRiseEffect()
    {
        isLoop = false;
        
        // 1, 9번 제외 재생 멈춤
        for (int i = 1; i < mainSources.Length - 1; i++)
            StopSource((MainBgmSourceType)i);

        audioDict[MainBgmSourceType.RiseEffect].Play();
        PlaySource(MainBgmSourceType.RiseEffect);
    }

    private void InitDictionary()
    {
        for (int i = 0; i < mainSources.Length; i++)
        {
            audioDict[(MainBgmSourceType)i] = mainSources[i];
        }

        const int startIndex = 2;
        for (int i = 0; i < dialogIndexes.Length; i++)
        {
            audioByIndexDict[dialogIndexes[i]] = (MainBgmSourceType)startIndex + i;
        }
    }
    
    private void PlaySource(MainBgmSourceType srcType)
    {
        var scr = audioDict[srcType];
        var duration = srcType == MainBgmSourceType.StrMelody2 ? 2f : 1f;
        duration = srcType == MainBgmSourceType.RiseEffect ? 8f : duration;
        EditorLog.LogWarning("Play Source : " + scr.gameObject.name);
        StartCoroutine(FadeInAudio(scr, duration));
    }

    private void StopSource(MainBgmSourceType srcType)
    {
        var scr = audioDict[srcType];
        var duration = srcType == MainBgmSourceType.StrMelody2 ? 2f : 1f;
        EditorLog.LogWarning("Stop Source : " + scr.gameObject.name);
        StartCoroutine(FadeOutAudio(scr, duration));
    }
    
    private void SetVolume(float volume)
    {
        bgmVolume = volume;
        for (int i = 0; i < mainSources.Length; i++)
        {
            var mainSource = mainSources[i];
            if (mainSource.volume != 0)
                mainSources[i].volume = bgmVolume;
        }
        
        loopEffectVolume = Mathf.Max(0.1f, bgmVolume * 0.9f);
        for (int i = 0; i < loopEffects.Length; i++)
            loopEffects[i].volume = loopEffectVolume;
    }
    
    private IEnumerator FadeInAudio(AudioSource src, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            src.volume = Mathf.Lerp(0f, bgmVolume, elapsed / duration);
            yield return null;
        }
        src.volume = bgmVolume;
    }

    private IEnumerator FadeOutAudio(AudioSource src, float duration)
    {
        float elapsed = 0f;       
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            src.volume = Mathf.Lerp(bgmVolume, 0f, elapsed / duration);
            yield return null;
        }                         
        src.volume = 0f;   
    }

    private void OnDestroy()
    {
        var dialogManager = Managers.Instance.DialogueManager;
        dialogManager.OnDialogStepEnd -= OnPlayStrMelody;
        dialogManager.OnDialogStepStart -= OnPlayAudioSources;

        var selectionPanel = Managers.Instance.UIManager.Get<UISelectionPanel>();
        if (selectionPanel == null) return;
        selectionPanel.OnFinalSelect -= OnPlayRiseEffect;
        Managers.Instance.SoundManager.SetBgmVolumeCallback -= SetVolume;
    }
}

public enum MainBgmSourceType
{
    Piano,
    StrMelody1,
    Marimba,
    StringPizz,
    GlockCelesta,
    Woodwinds,
    Bass,
    StrMelody2,
    RiseEffect,
}
