using UnityEngine;

//무조건 콜라이더2D가 존재해야함.
[RequireComponent(typeof(Collider2D))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("튜토리얼 인덱스")]
    [Tooltip("TutorialPopup의 entries 리스트에서 사용할 index")]
    [SerializeField] private int tutorialIndex;

    [Header("언제 보여줄지 (챕터)")]
    [SerializeField] private int requiredProgress;

    [Header("한 번만 보여줄지")]
    [SerializeField] private bool onlyOnce = true;

    private bool hasShown = false;

    private void Reset()
    {
        // 자동으로 트리거 설정
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasShown && onlyOnce) return;
        if (Managers.Instance.GameManager.ChapterProgress != requiredProgress) return;
        if (!col.CompareTag("Player")) return;

        // UIManager에 호출
        Managers.Instance.UIManager.Show<TutorialPopup>(tutorialIndex);

        if (onlyOnce) hasShown = true;
    }
}
