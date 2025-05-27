using Febucci.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UISelectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TypewriterByCharacter typewriter;
    
    public void DefaultInit(int index, Action<int> onClickAction, string text)
    {
        button.onClick.AddListener(() => onClickAction(index));
        gameObject.SetActive(true);
        typewriter.useTypeWriter = false;
        typewriter.ShowText(text);
    }

    public void HighlightInit(int index, Action<int> onClickAction, string text)
    {
        button.enabled = false;
        button.onClick.AddListener(() => onClickAction(index));
        
        typewriter.useTypeWriter = true;
        typewriter.ShowText(text);
        typewriter.onTextShowed.AddListener(() => button.enabled = true);
        
        gameObject.SetActive(true);
        typewriter.StartShowingText();
    }
    
    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        typewriter.onTextShowed.RemoveAllListeners();
    }
}
