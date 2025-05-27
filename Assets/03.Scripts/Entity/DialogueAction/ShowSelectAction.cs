using MainTable;

public class ShowSelectAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        var selectionPanel = Managers.Instance.UIManager.Show<UISelectionPanel>();
        selectionPanel.SetDefaultPanel(dialogData);
        
        if (isFirst) return;
        Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
    }
}