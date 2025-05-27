using Febucci.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChapterIntro : UIBase
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private TypewriterByCharacter typewriter;
    private const float FadeOutTime = 2f;
    private readonly Color fadeOutColor = new(0, 0, 0, 0f);
    private Action introEndCallback;

    public IEnumerator IntroCoroutine(bool isFirst, string text, Action callback = null)
    {
        introEndCallback = callback;
        if (!isFirst)
        {
            HideDirect();
            yield break;
        }

        Managers.Instance.GameManager.Player.Controller.IsControllable = false;
        
        typewriter.onTextShowed.AddListener(() => StartCoroutine(CompleteTextShowed()));
        typewriter.ShowText(text);
        typewriter.StartShowingText();
    }

    private IEnumerator CompleteTextShowed()
    {
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(Fade(Color.white, fadeOutColor, FadeOutTime, c => introText.color = c));
        yield return Fade(Color.black, fadeOutColor, FadeOutTime, c => backgroundImage.color = c);
        
        Managers.Instance.GameManager.Player.Controller.IsControllable = true;
        introEndCallback?.Invoke();
        HideDirect();
    }
    
    private IEnumerator Fade(Color from, Color to, float duration, Action<Color> applyColor)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            applyColor(Color.Lerp(from, to, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        applyColor(to);
    }

    private void OnDisable()
    {
        typewriter.onTextShowed.RemoveAllListeners();
        introEndCallback = null;
    }
}