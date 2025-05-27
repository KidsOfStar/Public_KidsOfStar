using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : UIBase
{
    [SerializeField] private RectTransform holeMask;
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform arrow;
    private const float Padding = 80f;
    
    private void OnEnable()
    {
        var player = Managers.Instance.GameManager.Player;
        player.Controller.LockPlayer();
        Time.timeScale = 0;
    }

    public void SetTarget(RectTransform targetUI)
    {
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.Communication);
        holeMask.position = targetUI.position;
        holeMask.sizeDelta = new Vector2(targetUI.sizeDelta.x + Padding, targetUI.sizeDelta.y + Padding);

        var maskLocal = holeMask.localPosition;
        arrow.localPosition = maskLocal + new Vector3(0, holeMask.sizeDelta.y * 0.5f, 0);
        arrow.localScale = IsOnLeftTargetUI(targetUI) ? Vector3.zero : new Vector3(-1f, 1f, 1f);
        
        StartCoroutine(AnimCoroutine());
    }

    private bool IsOnLeftTargetUI(RectTransform targetUI)
    {
        // UI 월드 위치 → 스크린 좌표
        var mainCam = Managers.Instance.GameManager.MainCamera;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCam, targetUI.position);
        
        // 왼쪽 절반이면 true, 아니면 false
        return screenPoint.x < (Screen.width * 0.5f);
    }

    private IEnumerator AnimCoroutine()
    {
        closeButton.onClick.AddListener(HideDirect);
        yield return new WaitForSecondsRealtime(2.5f);
        HideDirect();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        closeButton.onClick.RemoveAllListeners();
        
        var player = Managers.Instance.GameManager.Player;
        player.Controller.UnlockPlayer();
        Time.timeScale = 1;
    }
}
