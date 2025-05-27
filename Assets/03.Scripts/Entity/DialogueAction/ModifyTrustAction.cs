using MainTable;
using System;

// NextIndex 존재 가능
public class ModifyTrustAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        var gameManager = Managers.Instance.GameManager;
        
        var parts = dialogData.Param.Split(',');
        var targetChapter = gameManager.CurrentChapter;
        int trustValue = 0;
        
        if (parts.Length == 2)
        {
            if (!Enum.TryParse(parts[0], out targetChapter))
                EditorLog.LogError("ModifyTrustAction : Invalid chapter");
            
            if (!int.TryParse(parts[1], out trustValue))
                EditorLog.LogError("ModifyTrustAction : Invalid trust value");
        }
        else
        {
            if (!int.TryParse(parts[0], out trustValue))
                EditorLog.LogError("ModifyTrustAction : Invalid trust value");
        }
        
        Managers.Instance.GameManager.ModifyTrust(targetChapter, trustValue);

        // 첫번째 액션이라면 return
        if (isFirst) return;

        // NextIndex 출력 및 대사 출력 완료 콜백
        Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
        if (dialogData.NextIndex.Count <= 0)
        {
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
            return;
        }

        var nextIndex = dialogData.NextIndex[0];
        Managers.Instance.DialogueManager.SetCurrentDialogData(nextIndex);
    }
}