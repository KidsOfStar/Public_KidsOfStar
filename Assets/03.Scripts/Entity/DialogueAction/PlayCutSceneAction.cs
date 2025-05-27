using MainTable;
using System;

// NextIndex 존재 불가능
public class PlayCutSceneAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        if (!isFirst)
        {
            Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        }
        
        if (!Enum.TryParse<CutSceneType>(dialogData.Param, out var cutScene))
        {
            EditorLog.LogError($"PlayCutSceneAction : Invalid cutscene type: {dialogData.Param}");
            return;
        }

        if (Managers.Instance.CutSceneManager.IsCutScenePlaying)
            Managers.Instance.CutSceneManager.DestroyCurrentCutScene(true);
        
        Managers.Instance.CutSceneManager.PlayCutScene(cutScene);
    }
}
