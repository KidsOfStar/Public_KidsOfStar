using MainTable;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomActions
{
    private static Dictionary<CustomActionType, Action<string>> actionDict = new();

    public static void Init()
    {
        actionDict[CustomActionType.GoToEnding] = PlayEnding;
        actionDict[CustomActionType.MoveTo] = PlayerMoveTo;
        actionDict[CustomActionType.PlayCutScene] = PlayCutScene;
        actionDict[CustomActionType.Return] = DoNothing;
    }
    
    public static void ExecuteAction(SpecifiedAction actionData)
    {
        actionDict[actionData.Action].Invoke(actionData.Param);
    }
    
    // 컷씬과 자유상호작용 모두 쓰일 수 있음
    private static void PlayEnding(string param)
    {
        if (!Enum.TryParse<EndingType>(param, out var endingType))
        {
            EditorLog.LogError($"GoToEndingAction : Invalid load scene type: {param}");
            return;
        }
        
        if (Managers.Instance.CutSceneManager.IsCutScenePlaying)
            Managers.Instance.CutSceneManager.DestroyCurrentCutScene(true);
        
        Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        Managers.Instance.DialogueManager.OnDialogStepEnd?.Invoke(0);
        Managers.Instance.GameManager.TriggerEnding(endingType);
    }

    // 자유상호작용에만 쓰임
    private static void PlayerMoveTo(string param)
    {
        var split = param.Split(',');
        if (split.Length != 2)
        {
            EditorLog.LogError($"PlayerMoveToAction : Invalid param: {param}");
            return;
        }
        
        if (!float.TryParse(split[0], out var x) || !float.TryParse(split[1], out var y))
        {
            EditorLog.LogError($"PlayerMoveToAction : Invalid param: {param}");
            return;
        }
        
        Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        Managers.Instance.DialogueManager.OnDialogStepEnd?.Invoke(0);
        var player = Managers.Instance.GameManager.Player;
        player.transform.position = new Vector3(x, y, player.transform.position.z);
    }
    
    // 컷씬에만 쓰임
    private static void PlayCutScene(string param)
    {
        if (!Enum.TryParse<CutSceneType>(param, out var cutSceneType))
        {
            EditorLog.LogError($"PlayCutSceneAction : Invalid cutscene type: {param}");
            return;
        }
        
        if (Managers.Instance.CutSceneManager.IsCutScenePlaying)
        {
            Managers.Instance.CutSceneManager.DestroyCurrentCutScene(true);
        }
        
        Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        Managers.Instance.CutSceneManager.PlayCutScene(cutSceneType);
    }
    
    private static void DoNothing(string param)
    {
        Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        Managers.Instance.DialogueManager.OnDialogStepEnd?.Invoke(0);
    }
}
