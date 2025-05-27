using MainTable;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UISelectionPanel : UIBase
{
    [SerializeField] private UISelectButton[] selectButtons;
    private DialogData dialogData;
    private List<int> finalNextIndexes;
    private List<string> finalSelections;

    public Action OnFinalSelect { get; set; }

    public void SetDefaultPanel(DialogData dialog)
    {
        dialogData = dialog;
        var selectionList = dialogData.SelectOption;

        for (int i = 0; i < selectionList.Count; i++)
            selectButtons[i].DefaultInit(i, OnSelectButtonClick, dialogData.SelectOption[i]);
    }

    public void SetHighlightPanel(DialogData dialog)
    {
        dialogData = dialog;
        var selectionList = dialogData.SelectOption;

        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.ImportantChoice);
        for (int i = 0; i < selectionList.Count; i++)
            selectButtons[i].HighlightInit(i, OnSelectButtonClick, dialogData.SelectOption[i]);
    }

    public void SetFinalPanel(List<string> finalSelection, List<int> nextIndexes)
    {
        finalNextIndexes = nextIndexes;
        finalSelections = finalSelection;

        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.ImportantChoice);
        for (int i = 0; i < finalSelection.Count; i++)
        {
            selectButtons[i].HighlightInit(i, OnSelectButtonClick, finalSelection[i]);
        }
    }

    private void OnSelectButtonClick(int index)
    {
        int nextIndex = finalNextIndexes != null ? finalNextIndexes[index] : dialogData.NextIndex[index];

        HideDirect();
        if (nextIndex < 0)
        {
            // 데이터 매니저에서 특수인덱스 가져오기
            var specifiedAction = Managers.Instance.DataManager.GetSpecifiedActionData(nextIndex);
            // 다이얼로그 매니저에서 특수인덱스 실행하기
            CustomActions.ExecuteAction(specifiedAction);
            return;
        }

        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.ButtonPush);
        Managers.Instance.DialogueManager.SetCurrentDialogData(nextIndex);
        OnFinalSelect?.Invoke();

        RecordChapterChoice(index);
        RecordFinalChoice(index);
    }

    private void OnDisable()
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            selectButtons[i].gameObject.SetActive(false);
        }
    }

    private void RecordChapterChoice(int selectIndex)
    {
        if (dialogData == null) return;
        var analyticsManager = Managers.Instance.AnalyticsManager;
        if (dialogData.Index == 10022)
        {
            analyticsManager.RecordChapterEvent("Choice",
                                                ("Choice", dialogData.SelectOption[selectIndex]),
                                                ("Index", dialogData.Index));
            return;
        }

        if (dialogData.Index == 2032)
        {
            analyticsManager.RecordChapterEvent("Choice",
                                                ("Choice", dialogData.SelectOption[selectIndex]),
                                                ("Index", dialogData.Index));
            return;
        }

        if (dialogData.Index == 3039)
        {
            analyticsManager.RecordChapterEvent("Choice",
                                                ("Choice", dialogData.SelectOption[selectIndex]),
                                                ("Index", dialogData.Index));
            return;
        }

        if (dialogData.Index == 4024)
        {
            analyticsManager.RecordChapterEvent("Choice",
                                                ("Choice", dialogData.SelectOption[selectIndex]),
                                                ("Index", dialogData.Index));
        }
    }

    private void RecordFinalChoice(int selectIndex)
    {
        if (finalSelections == null) return;

        var analyticsManager = Managers.Instance.AnalyticsManager;
        analyticsManager.RecordChapterEvent("Choice",
                                            ("Choice", finalSelections[selectIndex]),
                                            ("Index", 5021));
    }
}