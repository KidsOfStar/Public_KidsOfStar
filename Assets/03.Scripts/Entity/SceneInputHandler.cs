using UnityEngine;

public class SceneInputHandler : DialogInputHandler
{
    private void Start()
    {
        Managers.Instance.DialogueManager.OnDialogStart += ActiveInputHandler;
        Managers.Instance.DialogueManager.OnDialogEnd += InactiveInputHandler;
        InactiveInputHandler();
    }
    
    private void Update()
    {
        if (Time.timeScale == 0) return;
        OnClick();
    }

    private void ActiveInputHandler()
    {
        if (Managers.Instance.CutSceneManager.IsCutScenePlaying) return;
        gameObject.SetActive(true);
    }
    
    private void InactiveInputHandler()
    {
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        Managers.Instance.DialogueManager.OnDialogStart -= ActiveInputHandler;
        Managers.Instance.DialogueManager.OnDialogEnd -= InactiveInputHandler;
    }
}
