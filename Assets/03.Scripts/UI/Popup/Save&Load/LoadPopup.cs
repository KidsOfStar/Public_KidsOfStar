public class LoadPopup : BaseSaveLoadPopup
{
    protected override void Start()
    {
        base.Start();
        isSaveMode = false;
        CreateSlotUI();
    }
    protected override void LoadSlot(int slotIndex)
    {
        var loadTop = Managers.Instance.UIManager.Show<LoadTop>();        //데이터가 있을 경우 로드
        loadTop.SetUp(slotIndex);
    }
}
