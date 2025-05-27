using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneEventTrigger : MonoBehaviour
{
    [Header("Trigger"), Tooltip("단순 TriggerCheck만 하는 경우 true")]
    [SerializeField] private bool canTrigger = false;
    
    [Header("Trigger Event"), Tooltip("특정 오브젝트의 Trigger에 반응합니다.")]
    [SerializeField] private int[] requiredDialogs;
    [SerializeField] private UnityEvent onTriggerEnterEvent;

    [Header("Specified Dialog Event"), Tooltip("특정 대화가 끝나면 반응합니다.")]
    [SerializeField] private int specifiedDialogIndex;
    [SerializeField] private UnityEvent onSpecifiedDialogEnd;
    
    private readonly Dictionary<int, bool> finishedDialog = new();
    private Action<int> onSpecifiedDialogCheck;

    public void Init()
    {
        // 딕셔너리 초기화
        for (int i = 0; i < requiredDialogs.Length; i++)
        {
            var index = requiredDialogs[i];
            finishedDialog[index] = false;
        }
        
        // 콜백에 등록
        Managers.Instance.DialogueManager.OnDialogStepEnd += CheckCurrentDialog;
        onSpecifiedDialogCheck += CheckSpecifiedDialog;
    }

    private void CheckCurrentDialog(int index)
    {
        if (finishedDialog.ContainsKey(index))
            finishedDialog[index] = true;
        
        onSpecifiedDialogCheck?.Invoke(index);
        CheckRequiredDialogFinished();
    }

    private void CheckRequiredDialogFinished()
    {
        // 하나라도 안본 대사가 있다면 return
        foreach (var value in finishedDialog.Values)
            if (!value) return;

        // 모두 봤다면 Trigger 가능
        canTrigger = true;
    }

    private void CheckSpecifiedDialog(int index)
    {
        if (index == specifiedDialogIndex)
            onSpecifiedDialogEnd?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canTrigger) return;
        if (!other.CompareTag("Player")) return;

        onTriggerEnterEvent?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Managers.Instance.DialogueManager.OnDialogStepEnd -= CheckCurrentDialog;
    }
}
