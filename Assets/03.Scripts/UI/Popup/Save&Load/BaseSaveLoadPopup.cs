using System.IO;
using UnityEngine;

public class BaseSaveLoadPopup : PopupBase
{
    public Transform content;       //슬롯들이 붙을 부모 오브젝트
    public SlotUI slotUI;           // 프리맵 연결하기
    public bool isSaveMode;         // 저장/불러오기 모드 전환
    public int maxSlotCount = 10;

    protected void CreateSlotUI()
    {
        for (int i = 0; i < maxSlotCount; i++)
        {
            var slot = Instantiate(slotUI, content); // 해당 위치에 프리팹 생성
            int slotIndex = i;

            string savename = GetSaveName(slotIndex);
            bool hasData = HasSaveData(slotIndex);

            slot.SetUp(slotIndex, savename, () =>
            {
                if (isSaveMode) SaveSlot(slotIndex, slot);
                else LoadSlot(slotIndex);
            });
            // 로드 모드에서 데이터 없으면 버튼 비활성화
            if (!isSaveMode && !hasData)
            {
                slot.SetInteractable(false);
            }
        }
    }

    protected virtual void SaveSlot(int slotIndex, SlotUI slot) { }

    protected virtual void LoadSlot(int slotIndex) { }

    private string GetSaveName(int slotIndex)
    {
        string path = Application.persistentDataPath + $"/SaveData{slotIndex}.json";

        if (!File.Exists(path)) return "비어 있음";

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);
        return data.saveName;
    }

    private bool HasSaveData(int slotIndex)
    {
        string path = Application.persistentDataPath + $"/SaveData{slotIndex}.json";
        return File.Exists(path);
    }
}
