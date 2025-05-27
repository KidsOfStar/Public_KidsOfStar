using MainTable;
using System;
using System.Collections.Generic;

public class FinalSelectAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        // TODO: SoundManager로 하이라이트 선택지 효과음 재생
        var path = Define.dataPath + "FinalSelectData";
        var finalSelection = Managers.Instance.ResourceManager.Load<FinalSelectData>(path);

        List<string> selectionList = new();
        List<int> nextIndexes = new();
        foreach (ChapterType chapter in Enum.GetValues(typeof(ChapterType)))
        {
            var selectData = finalSelection.GetSelection(chapter);
            if (selectData == null) continue;

            selectionList.Add(selectData.selectDialog);
            nextIndexes.Add(selectData.nextIndex);
        }
        
        var selectionPanel = Managers.Instance.UIManager.Show<UISelectionPanel>();
        selectionPanel.SetFinalPanel(selectionList, nextIndexes);
        
        if (isFirst) return;
        Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
    }
}