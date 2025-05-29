using System.Collections;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    [SerializeField] private AudioSource piano;
    [SerializeField] private AudioSource strMelody;
    [SerializeField] private AudioSource marimba;
    [SerializeField] private AudioSource rise;
    private float bgmVolume = 0.7f;

    private void Start()
    {
        // 초기 볼륨 설정
        piano.volume = 0f;
        strMelody.volume = 0f;
        marimba.volume = 0f;
        rise.volume = 0f;

        // BGM 테스트 시작
        StartCoroutine(Test());
    }
    
    private IEnumerator Test()
    {
        yield return new WaitForSeconds(5f);
        
        var dspStart = AudioSettings.dspTime + 0.5f;
        piano.PlayScheduled(dspStart);
        strMelody.PlayScheduled(dspStart);
        marimba.PlayScheduled(dspStart);
        
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FadeInAudio(piano, 1f));
        yield return new WaitForSeconds(3f);
        
        StartCoroutine(FadeInAudio(strMelody, 1f));
        yield return new WaitForSeconds(3f);
        
        StartCoroutine(FadeInAudio(marimba, 1f));
        yield return new WaitForSeconds(5f);
        
        StartCoroutine(FadeOutAudio(piano, 1f));
        StartCoroutine(FadeOutAudio(strMelody, 1f));
        StartCoroutine(FadeOutAudio(marimba, 1f));
        
        rise.Play();
        StartCoroutine(FadeInAudio(rise, 1f));
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
}
