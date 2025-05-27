using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DialogInputHandler : MonoBehaviour
{
    protected void OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            Managers.Instance.DialogueManager.OnClick?.Invoke();
        }
    }
}

public class CutSceneInputHandler : DialogInputHandler
{
    private bool isTalk;

    private void Start()
    {
        Managers.Instance.DialogueManager.OnDialogStart += IsTalk;
        Managers.Instance.DialogueManager.OnDialogEnd += IsNotTalk;
    }

    private void Update()
    {
#if TEST_BUILD
        // if (Input.GetMouseButtonDown(2))
        // {
        //     Managers.Instance.CutSceneManager.DestroyCurrentCutScene();
        // }
#endif
        
        if (!isTalk) return;

        OnClick();
    }

    private void IsTalk()
    {
        isTalk = true;
    }

    private void IsNotTalk()
    {
        isTalk = false;
    }

    private void OnDestroy()
    {
        Managers.Instance.DialogueManager.OnDialogStart -= IsTalk;
        Managers.Instance.DialogueManager.OnDialogEnd -= IsNotTalk;
    }
}