using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    [Header("페이드 효과에 사용될 이미지")]
    public Image fadePanel;
    [SerializeField]
    [Range(0.0f, 10f)] private float fadeTime;
    [SerializeField] private float durationTime;


    //Fade In(0,1)로
    //Fade Out(1,0)으로 호출
    public IEnumerator Fade(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;

            Color color = fadePanel.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadePanel.color = color;

            yield return null;
        }
    }
}