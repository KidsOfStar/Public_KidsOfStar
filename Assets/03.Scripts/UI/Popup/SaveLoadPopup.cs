using System.IO;
using UnityEngine;

public class SaveLoadPopup : PopupBase
{
    public Transform content;       //슬롯들이 붙을 부모 오브젝트
    public SlotUI slotUI;           // 프리맵 연결하기
    public bool isSaveMode = true;  // 저장/불러오기 모드 전환
    public int maxSlotCount = 10;

    protected override void Start()
    {
        base.Start();
        CreateSlotUI();
    }

    private void CreateSlotUI()
    {
        for (int i = 0; i < maxSlotCount; i++)
        {
            var slot = Instantiate(slotUI, content); // 해당 위치에 프리팹 생성
            int slotIndex = i;

            string savename = GetSaveName(slotIndex);

            slot.SetUp(slotIndex, savename, () =>
            {
                if (isSaveMode) SaveSlot(slotIndex, slot);
                else LoadSlot(slotIndex);
            });

        }
    }

    private void SaveSlot(int slotIndex, SlotUI slot)
    {
        Managers.Instance.SaveManager.Save(slotIndex, (saveName) => 
        {
            slot.UpdateSlotname(slotIndex, saveName);
        });
    }

    private void LoadSlot(int slotIndex)
    {
        Managers.Instance.SaveManager.Load(slotIndex);
    }

    private string GetSaveName(int slotIndex)
    {
        string path = Application.persistentDataPath + $"/SaveData{slotIndex}.json";

        if (!File.Exists(path)) return "empty slot";

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);
        return data.saveName;
    }
}
