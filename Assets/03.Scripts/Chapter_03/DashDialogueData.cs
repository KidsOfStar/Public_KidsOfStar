using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 시간 구간에 해당하는 대사 데이터를 담는 클래스
/// </summary>
[System.Serializable]
public class DialogueEntry
{
    public float dialoguesIndex;            // 기준 시간 (초 단위: 90, 150, 210 등)
    public List<string> jigimDialogues;     // 지김 NPC의 대사 목록 (문단 단위)
    public List<string> semyungDialogues;   // 세명 NPC의 대사 목록 (문단 단위)
}

/// <summary>
/// 여러 시간 구간의 DialogueEntry를 저장하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObject/DialogueData")]
public class DashDialogueData : ScriptableObject
{
    public List<DialogueEntry> entries; // 모든 시간 구간의 대사 목록

    /// <summary>
    /// 클리어 시간과 NPC 타입에 따라 해당하는 대사 리스트를 반환
    /// </summary>
    /// <param name="clearTime">플레이어 클리어 시간 (초)</param>
    /// <param name="npcType">대사를 출력할 NPC 타입</param>
    /// <returns>해당 NPC의 대사 리스트</returns>
    public List<string> GetDialogueByNpc(float clearTime, CharacterType npcType)
    {
        DialogueEntry selected = GetDialogueEntry(clearTime);
        return npcType switch
        {
            CharacterType.Jigim => selected.jigimDialogues,
            CharacterType.Semyung => selected.semyungDialogues,
            _ => new List<string>() // 알 수 없는 타입일 경우 빈 리스트 반환
        };
    }

    /// <summary>
    /// 주어진 시간보다 큰 첫 번째 DialogueEntry를 반환.
    /// 없을 경우 마지막 항목을 기본값으로 반환
    /// </summary>
    /// <param name="time">플레이어 클리어 시간</param>
    /// <returns>해당 시간에 맞는 DialogueEntry</returns>
    private DialogueEntry GetDialogueEntry(float time)
    {
        foreach (var entry in entries)
        {
            if (time < entry.dialoguesIndex)
            {
                return entry;
            }
        }
        return entries[entries.Count - 1]; // 기준 시간보다 큰 항목이 없으면 마지막 항목 반환
    }
}
