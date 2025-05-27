using System.Linq;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;

    // WarningType의 값을 순서대로 저장
    private WarningType[] warningQueue;

    // 현재 화면에 표시되고 있는 warningQueue내의 인덱스
    private int queueIndex;

    private ObjectIndicator objIndicator;

    public override void Opened(params object[] param)
    {
        base.Opened(param);

        warningQueue = param.OfType<WarningType>().ToArray();
        queueIndex = 0;

        objIndicator = param.OfType<ObjectIndicator>().FirstOrDefault();

        // 첫 경고 표시
        ShowCurrentWarning();
    }

    public void ShowCurrentWarning()
    {
        // 텍스트 숨겨두고, 해당 타입만 활성화
        messageText.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        confirmButton.onClick.RemoveAllListeners();

        // 현재 경고 타입 가져오기
        switch (warningQueue[queueIndex])
        {
            case WarningType.Squirrel:
                messageText.text = "세심하게 만져야 한다. 다른 방법이 없을까?";
                messageText.gameObject.SetActive(true);
                ButtonWithSfx(confirmButton, SfxSoundType.ButtonPush, OnConfirmPressed);
                break;

            case WarningType.BoxMissing:
                messageText.text = "박스를 문 앞으로 가져와야할 것 같다.";
                messageText.gameObject.SetActive(true);
                ButtonWithSfx(confirmButton, SfxSoundType.ButtonPush, OnConfirmPressed);
                break;

            case WarningType.BoxFalling:
                messageText.text = "상자가 떨어졌다. 다시 시도해보자.";
                messageText.gameObject.SetActive(true);
                ButtonWithSfx(confirmButton, SfxSoundType.ButtonPush, objIndicator.ResetPosition);
                break;

            case WarningType.Coweb:
                messageText.text = "거미줄을 없앨 수 없을까?";
                messageText.gameObject.SetActive(true);
                ButtonWithSfx(confirmButton, SfxSoundType.ButtonPush, OnConfirmPressed);
                break;

            case WarningType.BackMove:
                messageText.text = "사람 눈에 안 띄게 할 수 없을까?";
                messageText.gameObject.SetActive(true);
                ButtonWithSfx(confirmButton, SfxSoundType.ButtonPush, OnConfirmPressed);
                break;

            case WarningType.Ladder:
                messageText.text = "몰래 사용해야 할 것 같다.";
                messageText.gameObject.SetActive(true);
                ButtonWithSfx(confirmButton,SfxSoundType.ButtonPush, OnConfirmPressed);
                break;

            default:
                messageText.text = "…";
                messageText.gameObject.SetActive(true);
                break;
        }
    }

    private void OnConfirmPressed()
    {
        queueIndex++;

        // 다음 경고 표시
        if (queueIndex < warningQueue.Length)
            ShowCurrentWarning();

        // 모두 다 봤으면 팝업 닫기
        else
            Managers.Instance.UIManager.Hide<WarningPopup>();
    }

    private void ButtonWithSfx(Button btn, SfxSoundType sfx, UnityAction action)
    {
        btn.gameObject.SetActive(true);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            Managers.Instance.SoundManager.PlaySfx(sfx);
            action();
        });
    }

    public void SetScreenPosition(Vector3 worldPos)
    {
        if (Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        GetComponent<RectTransform>().position = screenPos;
    }
}

