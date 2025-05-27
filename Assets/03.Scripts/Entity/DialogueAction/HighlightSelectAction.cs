using MainTable;

public class HighlightSelectAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        // TODO: SoundManager로 하이라이트 선택지 효과음 재생
        var selectionPanel = Managers.Instance.UIManager.Show<UISelectionPanel>();
        selectionPanel.SetHighlightPanel(dialogData);
        
        if (isFirst) return;
        Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
    }
}