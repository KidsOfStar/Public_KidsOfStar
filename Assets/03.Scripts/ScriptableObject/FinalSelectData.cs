using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FinalSelectData", menuName = "ScriptableObject/FinalSelectData")]
public class FinalSelectData : ScriptableObject
{
    public FinalSelection[] finalSelections;

    public FinalSelection GetSelection(ChapterType chapterType)
    {
        if (chapterType is ChapterType.Chapter2 or ChapterType.Chapter3 or ChapterType.Chapter4)
        {
            if (!Managers.Instance.GameManager.EnoughTrustForEnding(chapterType))
                return null;
        }
        
        for (int i = 0; i < finalSelections.Length; i++)
        {
            var final = finalSelections[i];
            if (final.chapterType == chapterType)
                return final;
        }

        return null;
    }
}

[Serializable]
public class FinalSelection
{
    public ChapterType chapterType;
    public string selectDialog;
    public int nextIndex;
}
