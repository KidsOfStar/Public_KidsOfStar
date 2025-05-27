using MainTable;

// NextIndex 존재 불가능
public class DataSaveAction : IDialogActionHandler
{
    public void Execute(DialogData playerData, bool isFirst)
    {
        if (!isFirst)
        {
            Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
        }

        Managers.Instance.UIManager.Show<SavePopup>();
    }
}
