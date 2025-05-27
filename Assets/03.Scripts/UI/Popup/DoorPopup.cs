using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DoorPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;

    public override void Opened(params object[] param)
    {
        base.Opened(param);

        ShowCurrentWarning();

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(ClosePopup);
    }

    private void ShowCurrentWarning()
    {
        confirmButton.onClick.RemoveAllListeners();

        // 현재 경고 타입 가져오기
        messageText.text = "잠겨있다. 풀 방법이 없을까?";

        // 텍스트 숨겨두고, 해당 타입만 활성화
        messageText.gameObject.SetActive(true);
    }

    public void SetText(string text)
    {
        confirmButton.onClick.RemoveAllListeners();

        // 현재 경고 타입 가져오기
        messageText.text = text;

        // 텍스트 숨겨두고, 해당 타입만 활성화
        messageText.gameObject.SetActive(true);
    }

    private void ClosePopup()
    {
        Managers.Instance.UIManager.Hide<DoorPopup>();
    }

}

