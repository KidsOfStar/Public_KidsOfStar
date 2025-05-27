using MainTable;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : ISceneLifecycleHandler
{
    private readonly Dictionary<DialogActionType, IDialogActionHandler> dialogActionHandlers = new();
    private readonly Dictionary<CharacterType, IDialogSpeaker> sceneSpeakers = new();
    private readonly Dictionary<CharacterType, IDialogSpeaker> cutSceneSpeakers = new();
    private readonly Queue<string> dialogQueue = new();

    private DialogData currentDialogData;
    private UITextBubble textBubble;

    public Action OnClick { get; set; }
    public Action OnDialogStart { get; set; }
    public Action<int> OnDialogStepStart { get; set; }
    public Action OnDialogEnd { get; set; }
    public Action<int> OnDialogStepEnd { get; set; }
    private bool isCutScene;

    public DialogueManager()
    {
        dialogActionHandlers[DialogActionType.None] = new NoneAction();
        dialogActionHandlers[DialogActionType.ShowSelect] = new ShowSelectAction();
        dialogActionHandlers[DialogActionType.HighlightSelect] = new HighlightSelectAction();
        dialogActionHandlers[DialogActionType.FinalSelect] = new FinalSelectAction();
        dialogActionHandlers[DialogActionType.ModifyTrust] = new ModifyTrustAction();
        dialogActionHandlers[DialogActionType.DataSave] = new DataSaveAction();
        dialogActionHandlers[DialogActionType.PlayCutScene] = new PlayCutSceneAction();
        dialogActionHandlers[DialogActionType.LoadScene] = new LoadSceneAction();
        dialogActionHandlers[DialogActionType.UpdateProgress] = new UpdateProgressAction();
        dialogActionHandlers[DialogActionType.ChangeForm] = new ChangeFormAction();
        dialogActionHandlers[DialogActionType.GoToEnding] = new GoToEndingAction();

        CustomActions.Init();
    }

    // 말풍선 위치를 위해 씬에 배치된 npc들을 딕셔너리에 추가
    public void InitSceneNPcs(SceneNpc[] speakers)
    {
        foreach (var npc in speakers)
        {
            sceneSpeakers[npc.GetCharacterType()] = npc;
        }
    }

    // 말풍선 위치를 위해 컷씬에 배치된 npc들을 딕셔너리에 추가
    public void InitCutSceneNPcs(CutSceneNpc[] speakers)
    {
        foreach (var npc in speakers)
        {
            cutSceneSpeakers[npc.GetCharacterType()] = npc;
        }
    }

    // 플레이어는 씬마다 새로 생성하기 때문에 플레이어 생성 후 호출 할 함수
    public void SetPlayerSpeaker(IDialogSpeaker player)
    {
        sceneSpeakers[CharacterType.Dolmengee] = player;
    }

    // 현재 출력 할 대사 데이터를 초기화
    public void SetCurrentDialogData(int index)
    {
        currentDialogData = Managers.Instance.DataManager.GetDialogData(index);
        dialogQueue.Clear();
        if (currentDialogData == null)
        {
            EditorLog.LogError($"DialogueManager : Not found PlayerData with index: {index}");
            return;
        }

        // 인덱스가 10000 미만이면 컷씬으로 판단
        isCutScene = currentDialogData.Index < 10000;

        // 데이터의 대사 value 값을 @로 나누어 대사 큐에 넣음 
        var dialogs = currentDialogData.DialogValue.Split('@');
        foreach (var dialog in dialogs)
            dialogQueue.Enqueue(dialog);

        // 대사 출력 시작 이벤트 호출
        OnDialogStart?.Invoke();
        OnDialogStepStart?.Invoke(currentDialogData.Index);

        // 큐의 Count가 0이 될 때까지 대사를 출력
        if (dialogQueue.Count > 0)
        {
            ShowDialog(dialogQueue.Dequeue(), currentDialogData.Character);
        }
        // 더 이상 출력 할 대사가 없다면 대사 출력 종료 이벤트 호출
        else OnDialogEnd?.Invoke();
    }

    public void SetInteractObjectDialog(string dialog)
    {
        currentDialogData = null;
        dialogQueue.Clear();
        isCutScene = false;

        // 대사 출력 시작 이벤트 호출
        OnDialogStart?.Invoke();
        ShowDialog(dialog, CharacterType.Dolmengee);
    }

    // 대사를 라인별로 나눠서 TextBubble UI에 전달하여 출력
    private void ShowDialog(string dialog, CharacterType character)
    {
        var npc = isCutScene ? cutSceneSpeakers[character] : sceneSpeakers[character];
        Transform bubbleTr = npc.GetBubbleTr();

        // var localPos = WorldToCanvasPosition(bubblePos);
        var formattedDialog = dialog.Replace("\\n", "\n");

        textBubble.SetActive(true);
        textBubble.SetDialog(formattedDialog, bubbleTr);
    }

    // 말풍선 쪽에서 사용하는 함수
    public void OnDialogLineComplete()
    {
        // 메인 대사 테이블이 아닌 상호작용 오브젝트 대사일 경우
        if (currentDialogData == null)
        {
            textBubble.HideDirect();
            OnDialogEnd?.Invoke();
            return;
        }

        // 모든 대사가 출력되었는지 체크
        if (dialogQueue.Count > 0)
        {
            // 다음 대사가 남아있다면 대사 큐에서 다음 대사를 꺼내서 출력
            ShowDialog(dialogQueue.Dequeue(), currentDialogData.Character);
        }
        else
        {
            textBubble.HideDirect();

            // 대사 출력이 끝났다면 액션 타입에 따라 다이얼로그 액션 실행
            dialogActionHandlers[currentDialogData.FirstAction].Execute(currentDialogData, true);
            dialogActionHandlers[currentDialogData.SecondAction].Execute(currentDialogData, false);
        }
    }

    public void InvokeOnDialogStepEnd()
    {
        OnDialogStepEnd?.Invoke(currentDialogData.Index);
    }

    public void OnSceneLoaded()
    {
        const string path = Define.uiPath + "UITextBubble";
        textBubble = Managers.Instance.ResourceManager.Instantiate<UITextBubble>(path);
        textBubble.InitCamera();
        textBubble.HideDirect();
    }

    public void OnSceneUnloaded()
    {
        sceneSpeakers.Clear();
        cutSceneSpeakers.Clear();
        textBubble = null;
    }
}