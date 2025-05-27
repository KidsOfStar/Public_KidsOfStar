using Febucci.UI;
using UnityEngine;

public class UITextBubble : UIBase
{
    [Header("Text Bubble")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform rectTr;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private float clickIgnoreTime = 0.1f;

    private Coroutine dialogCoroutine;
    
    private bool isTyping = false;
    private float dialogStartTime;

    public void InitCamera()
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Managers.Instance.GameManager.MainCamera;
        rectTr.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    
    public void SetDialog(string dialog, Transform bubbleTr)
    {
        ShowDialogue(dialog);
        rectTr.SetParent(bubbleTr);
        rectTr.localPosition = Vector3.zero;
        dialogStartTime = Time.time;
    }

    private void ShowDialogue(string dialog)
    {
        isTyping = true;
        typewriter.ShowText(dialog);
        
        typewriter.onCharacterVisible.AddListener(_ => CheckSkipTyping());
        typewriter.onTextShowed.AddListener(() => isTyping = false);
        typewriter.StartShowingText();
    }

    private void CheckSkipTyping()
    {
        if (!isTyping)
            typewriter.SkipTypewriter();
    }
    
    private void SkipTyping()
    {
        if (Time.time - dialogStartTime < clickIgnoreTime)
            return;
        
        if (isTyping)
            isTyping = false;
        else
        {
            typewriter.onCharacterVisible.RemoveAllListeners();
            typewriter.onTextShowed.RemoveAllListeners();
            Managers.Instance.DialogueManager.OnDialogLineComplete();
        }
    }

    public override void HideDirect()
    {
        rectTr.SetParent(null);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        dialogStartTime = 0f;
        Managers.Instance.DialogueManager.OnClick -= SkipTyping;
        Managers.Instance.DialogueManager.OnClick += SkipTyping;
    }

    private void OnDisable()
    {
        typewriter.ShowText(string.Empty);
        Managers.Instance.DialogueManager.OnClick -= SkipTyping;
    }
}
