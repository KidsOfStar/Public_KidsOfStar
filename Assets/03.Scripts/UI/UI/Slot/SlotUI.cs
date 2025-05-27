using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 저장/불러오기 슬롯 UI를 담당하는 클래스
/// </summary>
public class SlotUI : MonoBehaviour
{
    [Header("Slot UI")]
    public TextMeshProUGUI slotName;  // 슬롯 이름을 표시하는 텍스트 (예: Slot1 : 2025-04-15 12:00:00)
    private Button slotButton;        // 슬롯 자체에 연결된 버튼 컴포넌트
    /// <summary>
    /// 슬롯 UI를 초기화하고 클릭 이벤트를 설정합니다.
    /// </summary>
    /// <param name="index">슬롯 인덱스 (0부터 시작)</param>
    /// <param name="saveName">저장된 슬롯 이름 또는 날짜</param>
    /// <param name="onClicked">클릭 시 실행할 콜백</param>
    public void SetUp(int index, string saveName, System.Action onClicked)
    {
        // 슬롯 텍스트 설정 (표시할 때는 1부터 시작하는 숫자 사용)
        slotName.text = $"Slot{index + 1} : {saveName}";

        // 버튼 컴포넌트 연결
        slotButton = GetComponent<Button>();

        // 기존 리스너 제거 후 새로 등록
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => onClicked?.Invoke());
    }

    public void SetInteractable(bool interactable)
    {
        slotButton.interactable = interactable;
    }

    /// <summary>
    /// 저장 후 슬롯 이름을 업데이트합니다.
    /// </summary>
    /// <param name="index">슬롯 인덱스</param>
    /// <param name="saveName">저장된 이름 또는 타임스탬프</param>
    internal void UpdateSlotname(int index, string saveName)
    {
        slotName.text = $"Slot{index + 1} : {saveName}";
    }
}
