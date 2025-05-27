using Febucci.UI;
using System.Collections;
using TMPro;
using UnityEngine;

public class UILoading : MonoBehaviour
{
    [Header("Loading Data")]
    [SerializeField] private LoadingData androidData;
    [SerializeField] private LoadingData webGLData;

    [Header("Loading UI")]
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private TypewriterByCharacter tooltipText;
    [SerializeField] private GameObject title;
    [SerializeField] private string[] tooltips;

    private readonly WaitForSeconds tooltipWaitTime = new(2f);
    private LoadingData currentLoadingData;
    private int currentTooltipIndex;

    private void Start()
    {
        if (androidData == null || webGLData == null)
        {
            EditorLog.LogError("LoadingData is not assigned in the inspector.");
            return;
        }

        currentLoadingData = webGLData;
#if UNITY_EDITOR
        currentLoadingData = webGLData;
#elif UNITY_ANDROID
        currentLoadingData = androidData;
#elif UNITY_WEBGL
        currentLoadingData = webGLData;
#endif

        SetRandomBackground();
        StartCoroutine(TooltipCoroutine());
    }

    private IEnumerator TooltipCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        
        title.SetActive(true);
        
        if (currentLoadingData.Tooltips.Length == 0)
        {
            EditorLog.LogError("No tooltips found in LoadingData.");
            yield break;
        }

        for (int i = 0; i < currentLoadingData.Tooltips.Length; i++)
        {
            var tooltipData = currentLoadingData.Tooltips[i];
            var nextScene = Managers.Instance.SceneLoadManager.NextSceneToLoad;
            if (tooltipData.sceneType != nextScene) continue;

            tooltips = tooltipData.tooltips;
            break;
        }

        if (tooltips == null || tooltips.Length == 0)
        {
            EditorLog.LogWarning("No tooltips found for the current scene.");
            yield break;
        }

        while (true)
        {
            if (currentTooltipIndex >= tooltips.Length)
            {
                currentTooltipIndex = 0;
            }

            tooltipText.ShowText(tooltips[currentTooltipIndex]);
            tooltipText.StartShowingText();
            currentTooltipIndex++;
            yield return tooltipWaitTime;
        }
    }

    private void SetRandomBackground()
    {
        if (currentLoadingData.Backgrounds.Length == 0)
        {
            EditorLog.LogError("No background images found in LoadingData.");
            return;
        }

        int randomIndex = Random.Range(0, currentLoadingData.Backgrounds.Length);
        backgroundImage.sprite = currentLoadingData.Backgrounds[randomIndex];
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}