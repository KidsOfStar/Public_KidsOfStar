using MainTable;

// 첫번째 액션인지, 두번째 액션인지
public interface IDialogActionHandler
{
    void Execute(DialogData dialogData, bool isFirst);
}

// NextIndex 존재 가능
public class NoneAction : IDialogActionHandler
{
    public void Execute(DialogData dialogData, bool isFirst)
    {
        // 두번째 액션에서 end콜백이나 다음 대화를 처리하기 때문에
        // 첫번째 액션이라면 아무것도 하지 않음
        if (isFirst) return;
        Managers.Instance.DialogueManager.InvokeOnDialogStepEnd();

        // NextIndex가 없으면 대화 종료
        if (dialogData.NextIndex.Count <= 0)
        {
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
            return;
        }
        
        // TODO: Refactor: IDialogActionHandler
        // NextIndex가 지정 액션 인덱스라면?
        var nextIndex = dialogData.NextIndex[0];
        if (nextIndex < 0)
        {
            // 데이터 매니저에서 특수인덱스 가져오기
            var specifiedAction = Managers.Instance.DataManager.GetSpecifiedActionData(nextIndex);
            // 다이얼로그 매니저에서 특수인덱스 실행하기
            CustomActions.ExecuteAction(specifiedAction);
            
            Managers.Instance.DialogueManager.OnDialogEnd?.Invoke();
            return;
        }

        Managers.Instance.DialogueManager.SetCurrentDialogData(nextIndex);
    }
}