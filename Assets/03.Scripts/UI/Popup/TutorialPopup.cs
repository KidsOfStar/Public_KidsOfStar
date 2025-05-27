using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TutorialEntry
{
    [Header("팝업에 포함될 이미지")]
    public List<Sprite> images;

    [Header("표시할 제목")]
    public string title;

    [TextArea(3, 10)]
    [Header("설명 텍스트")]
    public string description;

    [Header("커스텀을 위한 Prefab(빈칸일 경우 스프라이트를 사용")]
    public GameObject customPrefab;

    [Header("커스텀 이미지 크기 (옵션)")]
    public Vector2 imageSize;         // Vector2.zero일 경우 자동 비율 적용
}

public class TutorialPopup : PopupBase
{
    [Header("Background Panel (숨기기용)")]
    [SerializeField] private GameObject backgroundPanel;

    public event Action OnClosed;

    [Header("이미지")]
    [SerializeField] private Image popupImage;

    [Header("커스텀 콘텐츠를 넣을 컨테이너")]
    [SerializeField] private RectTransform customContainer;

    [Header("텍스트 바인딩")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("튜토리얼 리스트")]
    [SerializeField] private List<TutorialEntry> entries;

    public override void Opened(object[] param)
    {
        base.Opened(param);

        Managers.Instance.GameManager.Player.Controller.LockPlayer();


        // param[0]에 tutorialIndex를 int로 넘겨받는다
        if (param.Length == 0 || !(param[0] is int idx))
        {
            EditorLog.LogWarning("TutorialPopup: 잘못된 파라미터");
            ClosePopup();
            return;
        }

        if (idx < 0 || idx >= entries.Count)
        {
            EditorLog.LogWarning($"TutorialPopup: index out of range ({idx})");
            ClosePopup();
            return;
        }
        var entry = entries[idx];

        titleText.text = entry.title;
        descriptionText.text = entry.description;

        // 기존 콘텐츠 초기화
        if (popupImage) popupImage.gameObject.SetActive(false);

        foreach (Transform child in customContainer)
        {
            Destroy(child.gameObject);
        }

        if (entry.customPrefab != null)
        {
            if (backgroundPanel) backgroundPanel.SetActive(false);

            titleText.gameObject.SetActive(false);
            descriptionText.gameObject.SetActive(false);

            // 커스텀 콘텐츠 인스턴스화
            var go = Instantiate(entry.customPrefab, customContainer);
            if (go.TryGetComponent<RectTransform>(out var childRt))
            {
                childRt.SetParent(customContainer, false);
                childRt.anchorMin = Vector2.zero;
                childRt.anchorMax = Vector2.one;
                childRt.offsetMin = Vector2.zero;
                childRt.offsetMax = Vector2.zero;
            }

            if (closeBtn) closeBtn.transform.SetAsLastSibling();
            return;
        }

        // customPrefab 없으면 이미지 슬롯 활용
        if (popupImage && entry.images != null && entry.images.Count > 0)
        {
            var sprite = entry.images[0];
            popupImage.gameObject.SetActive(true);
            popupImage.sprite = sprite;

            var pos = popupImage.rectTransform;
        }
    }

    public override void HideDirect()
    {
        Managers.Instance.GameManager.Player.Controller.UnlockPlayer();
        
        base.HideDirect();
        // 팝업이 완전히 닫히면 이벤트 발행
        OnClosed?.Invoke();
        OnClosed = null; // 구독 해제

    }

    public void ClosePopup()
    {
        HideDirect();
    }
}
