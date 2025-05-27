public class SavePopup : BaseSaveLoadPopup
{
    protected override void Start()
    {
        base.Start();       // Close버튼 가져오
        isSaveMode = true;  // Save모드 
        CreateSlotUI();     // 저장할 프리맵 생성
    }

    protected override void SaveSlot(int slotIndex, SlotUI slot)
    {
        var saveTop = Managers.Instance.UIManager.Show<SaveTop>();
        saveTop.SetUp(slotIndex, slot);
    }
}
