using UnityEngine.UI;

public class SaveTop : PopupBase
{
    private int slotIndex { get; set; }
    public Button checkButton;
    private SlotUI slot;

    public void SetUp(int index, SlotUI slotUI)
    {
        slotIndex = index;
        slot = slotUI;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        checkButton.onClick.AddListener(OnCheck);
    }
    private void Save()
    {
        Managers.Instance.SaveManager.Save(slotIndex, (saveName) =>
        {
            slot.UpdateSlotname(slotIndex, saveName);   // 저장 슬롯 업데이트
        });
    }

    public void OnCheck()
    {
        // 씬 불러오기
        Save();
        HideDirect();
    }
}
