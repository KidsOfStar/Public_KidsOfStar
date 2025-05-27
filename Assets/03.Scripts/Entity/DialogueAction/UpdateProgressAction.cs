using MainTable;
using UnityEngine;

// NextIndex 존재 불가능
public class UpdateProgressAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        if (!isFirst)
        {
            Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        }

        Managers.Instance.GameManager.UpdateProgress();
    }
}