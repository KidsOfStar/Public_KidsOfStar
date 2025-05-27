using System.Collections;
using UnityEngine;

public class FormBlocker : MonoBehaviour
{
    [SerializeField] private GameObject bubbleTextPrefab;   // 말풍선 프리팹
    private GameObject bubbleTextInstance;                  // 생성된 프리팹 인스턴스

    private Coroutine warningCoroutine;                     // 경고 코루틴

    // 동물 폼 목록 (모두 소문자)
    [SerializeField] private PlayerFormType dangerFormMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnInteraction();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnInteraction();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
            }

            if (bubbleTextInstance != null)
            {
                Destroy(bubbleTextInstance);
                bubbleTextInstance = null;
            }
        }
    }

    private void OnInteraction()
    {
        if (warningCoroutine != null) return;

        var player = Managers.Instance.GameManager.Player;
        var currentForm = player?.FormControl?.CurFormData;

        if (currentForm == null) return;

        if ((dangerFormMask & currentForm.playerFormType) != 0)
        {
            warningCoroutine = StartCoroutine(ShowWarning());
        }
    }

    private IEnumerator ShowWarning()
    {
        var player = Managers.Instance.GameManager.Player;

        if (bubbleTextPrefab == null) yield break;

        bubbleTextInstance = Instantiate(bubbleTextPrefab, player.transform);
        bubbleTextInstance.transform.localPosition = new Vector3(0, 2f, 0);

        var bubbleText = bubbleTextInstance.GetComponentInChildren<DoorPopup>();

        if (bubbleText != null)
        {
            bubbleText.SetText("정체를 들킬 것 같다.");
            Managers.Instance.SoundManager.PlaySfx(SfxSoundType.Beep);
        }
        yield return new WaitForSeconds(1f);

        if (bubbleText != null)
        {
            bubbleText.SetText("당장 피해야 한다.");
            Managers.Instance.SoundManager.PlaySfx(SfxSoundType.Beep);

        }
        yield return new WaitForSeconds(1f);

        if (bubbleText != null)
        {
            bubbleText.SetText("위험하다.");
            Managers.Instance.SoundManager.PlaySfx(SfxSoundType.Beep);
        }
        yield return new WaitForSeconds(1f);

        TriggerCaughtEnding();
    }

    private void TriggerCaughtEnding()
    {
        Managers.Instance.GameManager.TriggerEnding(EndingType.Detection);
    }
}
