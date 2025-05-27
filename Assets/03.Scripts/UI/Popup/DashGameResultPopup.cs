using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DashGameResultPopup : PopupBase
{
    [Header("Dialogue UI")]
    [SerializeField] private TextMeshProUGUI jigimText;
    [SerializeField] private TextMeshProUGUI semyungText;

    [Header("Dialogue Data")]
    [SerializeField] private DashDialogueData dialogueDatabase;

    public Action OnDialogEnd { get; set; }

    private int currentLineIndex = 0;
    private List<string> currentDialogLines;
    private CharacterType currentNpcType;

    public override void Opened(params object[] param)
    {
        base.Opened(param);

        if (param.Length < 2 || !(param[0] is float index) || !(param[1] is CharacterType npcType))
        {
            return;
        }

        currentNpcType = npcType;
        currentDialogLines = dialogueDatabase.GetDialogueByNpc(index, npcType);


        // NPC 위치 가져오기
        Transform npcTransform = GetNpcTransform(npcType);
        if (npcTransform != null)
        {
            Vector2 uiPosition = WorldToCanvasPosition(npcTransform.position);
            GetComponent<RectTransform>().anchoredPosition = uiPosition;
        }
        currentLineIndex = 0;
    }

    public void OnClickDialogue()
    {
        if (currentDialogLines == null) return;

        if (currentLineIndex < currentDialogLines.Count)
        {
            ShowCurrentLine();
        }
        else
        {
            Managers.Instance.UIManager.Hide<DashGameResultPopup>();
            Managers.Instance.GameManager.Player.Controller.UnlockPlayer(); // 플레이어 잠금 해제
            Managers.Instance.CutSceneManager.PlayCutScene(CutSceneType.FieldNormalLife); // 컷씬 재생
            Managers.Instance.DialogueManager.OnDialogEnd -= OnClickDialogue; // 대사 완료 이벤트 해제
            OnDialogEnd?.Invoke(); // 대사 완료 이벤트 호출
        }
    }

    private void ShowCurrentLine()
    {
        if (currentDialogLines == null || currentLineIndex >= currentDialogLines.Count)
        {
            return;
        }

        Managers.Instance.AnalyticsManager.SendFunnel("21");

        string line = currentDialogLines[currentLineIndex];

        if (currentNpcType == CharacterType.Jigim)
        {
            jigimText.text = line;
            semyungText.text = string.Empty;
        }
        else if (currentNpcType == CharacterType.Semyung)
        {
            jigimText.text = string.Empty;
            semyungText.text = line;
        }
        currentLineIndex++;
    }

    public Transform GetNpcTransform(CharacterType npcType)
    {
        var npc = FindObjectsOfType<SceneNpc>().FirstOrDefault(n => n.GetCharacterType() == npcType);
        return npc != null ? npc.transform : null;
    }

    private Vector2 WorldToCanvasPosition(Vector3 worldPosition)
    {
        var cam = Managers.Instance.GameManager.MainCamera;
        var canvasTr = Managers.Instance.UIManager.CanvasRectTr;
        Vector2 viewportPosition = cam.WorldToViewportPoint(worldPosition);
        Vector2 canvasSize = canvasTr.sizeDelta;
        return new Vector2((viewportPosition.x - 0.5f) * canvasSize.x, (viewportPosition.y - 0.35f) * canvasSize.y);
    }
}
