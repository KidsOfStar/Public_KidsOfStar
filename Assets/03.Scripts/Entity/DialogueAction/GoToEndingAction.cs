using MainTable;
using System;

// NextIndex 존재 불가능
public class GoToEndingAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        if (!isFirst)
        {
            Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        }

        if (!Enum.TryParse<EndingType>(dialogData.Param, out var endingType))
        {
            EditorLog.LogError($"GoToEndingAction : Invalid load scene type: {dialogData.Param}");
            return;
        }

        Managers.Instance.CutSceneManager.DestroyCurrentCutScene(true);
        Managers.Instance.GameManager.TriggerEnding(endingType);
    }
}
