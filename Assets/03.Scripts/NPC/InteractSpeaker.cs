using System.Collections.Generic;
using UnityEngine;

public abstract class InteractSpeaker : MonoBehaviour
{
    [field: SerializeField] public ChapterType ChapterType { get; private set; }
    [field: SerializeField] public CharacterType CharacterType { get; private set; }
    [field: SerializeField] public Transform BubbleTr { get; private set; }
    [SerializeField] private bool dontInit = false;

    private readonly Dictionary<int, int> dialogByProgress = new();
    private readonly Dictionary<int, int> requiredDialogByProgress = new();
    private GameObject exclamationIcon;
    private SkillBTN skillPanel;

    public void Init()
    {
        if (dontInit) return;

        var dict = Managers.Instance.DataManager.GetNpcDataDict();
        var startRange = ((int)Managers.Instance.GameManager.CurrentChapter + 1) * 100;
        var endRange = startRange + 100;

        for (int i = startRange; i < endRange; i++)
        {
            if (!dict.TryGetValue(i, out var npcData)) continue;
            if (npcData.Character != CharacterType) continue;

            dialogByProgress.Add(npcData.Progress, npcData.DialogIndex);
        }

        InitRequiredDialog();
        Managers.Instance.GameManager.OnProgressUpdated += CheckExistRequiredDialog;
        Managers.Instance.DialogueManager.OnDialogStepEnd += DespawnExclamationIcon;
        
        var playerBtn = Managers.Instance.UIManager.Show<PlayerBtn>();
        skillPanel = playerBtn.skillPanel;
        playerBtn.HideDirect();
    }

    private void InitRequiredDialog()
    {
        // 필수 대사 인덱스 리스트를 읽어옴
        var indexList = Managers.Instance.DataManager.GetRequiredIndex(ChapterType);

        if (indexList == null || indexList.Length == 0)
        {
            Debug.LogWarning($"No required dialog index found for {ChapterType}");
            return;
        }
        
        for (int i = 0; i < indexList.Length; i++)
        {
            var indexData = indexList[i];

            // 자신의 캐릭터 타입과 맞다면 딕셔너리에 저장
            if (indexData.characterType == CharacterType)
                requiredDialogByProgress[indexData.progress] = indexData.index;
        }
    }

    // 프로그레스가 업데이트 되는 이벤트에 등록
    private void CheckExistRequiredDialog()
    {
        var currentProgress = Managers.Instance.GameManager.ChapterProgress;
        foreach (var pair in requiredDialogByProgress)
        {
            if (pair.Key != currentProgress) continue;

            // 느낌표 띄우기
            exclamationIcon = Managers.Instance.PoolManager.Spawn(Define.requiredIconKey, BubbleTr);
            exclamationIcon.transform.localPosition = Vector3.zero;
        }
    }

    private void DespawnExclamationIcon(int index)
    {
        if (!this)
        {
            Managers.Instance.GameManager.OnProgressUpdated -= CheckExistRequiredDialog;
            Managers.Instance.DialogueManager.OnDialogStepEnd -= DespawnExclamationIcon;
            Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButtonCallback;
            EditorLog.Log("Destroy InteractSpeaker But Not Destroy");
            return;
        }
        
        if (BubbleTr.childCount == 0) return;
        
        foreach (var value in requiredDialogByProgress.Values)
        {
            if (index != value) continue;
            if (!exclamationIcon) return;

            Managers.Instance.PoolManager.Despawn(exclamationIcon);
            exclamationIcon = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 상호작용 버튼 이벤트에 등록
        if (!other.CompareTag("Player")) return;
        if (!dialogByProgress.TryGetValue(Managers.Instance.GameManager.ChapterProgress, out var dialog))
            return;
        if (!skillPanel) return;

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
        Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButtonCallback;
        Managers.Instance.DialogueManager.OnDialogEnd += ShowInteractionButtonCallback;

        // 대사 출력
        ShowDialog();
    }

    private void ShowDialog()
    {
        var key = Managers.Instance.GameManager.ChapterProgress;
        if (!dialogByProgress.TryGetValue(key, out int dialogIndex))
        {
            Debug.LogWarning($"No dialog found for progress {key}");
            return;
        }

        Managers.Instance.DialogueManager.SetCurrentDialogData(dialogIndex);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 상호작용 버튼 이벤트에 해제
        if (!other.CompareTag("Player")) return;
        if (!dialogByProgress.TryGetValue(Managers.Instance.GameManager.ChapterProgress, out var dialog))
            return;
        if (!skillPanel) return;

        skillPanel.ShowInteractionButton(false);
        skillPanel.OnInteractBtnClick -= OnInteract;
        Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButtonCallback;
    }

    private void ShowInteractionButtonCallback()
    {
        skillPanel.ShowInteractionButton(true);
        skillPanel.OnInteractBtnClick -= OnInteract;
        skillPanel.OnInteractBtnClick += OnInteract;
    }

    private void OnDestroy()
    {
        Managers.Instance.GameManager.OnProgressUpdated -= CheckExistRequiredDialog;
        Managers.Instance.DialogueManager.OnDialogStepEnd -= DespawnExclamationIcon;
        Managers.Instance.DialogueManager.OnDialogEnd -= ShowInteractionButtonCallback;
    }
}