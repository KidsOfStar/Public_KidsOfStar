using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    [SerializeField] private string objectName;
    private readonly Dictionary<int, string> dialogByProgress = new();
    private SkillBTN skillPanel;

    public void Init()
    {
        if (objectName == string.Empty) return;
        
        var dialogTable = Managers.Instance.DataManager.GetInteractionDataDict();
        foreach (var interactionData in dialogTable.Values)
        {
            if (interactionData.Object == objectName)
            {
                dialogByProgress.Add(interactionData.Progress, interactionData.PlayerDialog);
            }
        }

        SetSkillBtn();
    }

    private void SetSkillBtn()
    {
        var playerBtn = Managers.Instance.UIManager.Show<PlayerBtn>();
        skillPanel = playerBtn.skillPanel;
        playerBtn.HideDirect();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (objectName == string.Empty) return;
        
        // 상호작용 버튼 이벤트에 등록
        if (!other.CompareTag("Player")) return;

        skillPanel.OnInteractBtnClick -= OnInteract;
        skillPanel.OnInteractBtnClick += OnInteract;
        skillPanel.ShowInteractionButton(true);
    }
    
    private void OnInteract()
    {
        // 상호작용 버튼 이벤트에 해제
        skillPanel.OnInteractBtnClick -= OnInteract;
        skillPanel.ShowInteractionButton(false);

        // 대사 종료 이벤트에 등록
        Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButton;
        Managers.Instance.DialogueManager.OnDialogEnd += ShowInteractionButton;

        // 대사 출력
        ShowDialog();
    }

    private void ShowDialog()
    {
        var currentProgress = Managers.Instance.GameManager.ChapterProgress;
        if (dialogByProgress.TryGetValue(currentProgress, out var dialog))
        {
            Managers.Instance.DialogueManager.SetInteractObjectDialog(dialog);
            return;
        }
        
        if (dialogByProgress.TryGetValue(0, out var defaultDialog))
            Managers.Instance.DialogueManager.SetInteractObjectDialog(defaultDialog);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (objectName == string.Empty) return;
        
        // 상호작용 버튼 이벤트에 해제
        if (!other.CompareTag("Player")) return;

        skillPanel.ShowInteractionButton(false);
        skillPanel.OnInteractBtnClick -= OnInteract;
        Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButton;
    }

    private void ShowInteractionButton()
    {
        skillPanel.ShowInteractionButton(true);
        skillPanel.OnInteractBtnClick += OnInteract;
    }

    private void OnDestroy()
    {
        if (skillPanel != null)
        {
            skillPanel.OnInteractBtnClick -= OnInteract;
        }
        
        Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButton;
    }
}