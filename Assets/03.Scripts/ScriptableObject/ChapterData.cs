using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "ScriptableObject/ChapterData")]
public class ChapterData : ScriptableObject
{
    public ChapterProgress[] chapterProgresses;
}

[Serializable]
public class ChapterProgress
{
    public ChapterType chapterType;
    public int maxProgress;
}