using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequiredIndexData", menuName = "ScriptableObject/RequiredIndexData")]
public class RequiredIndexData : ScriptableObject
{
    public Dictionary<ChapterType, RequiredIndex[]> requiredIndexDict = new();
    [Header("Chapter 1")]
    public RequiredIndex[] chapter1List;

    [Header("Chapter 2")]
    public RequiredIndex[] chapter2List;
    
    [Header("Chapter 3")]
    public RequiredIndex[] chapter3List;
    
    [Header("Chapter 4")]
    public RequiredIndex[] chapter4List;

    [Header("Chapter 5")]
    public RequiredIndex[] chapter5List;

    public void Init()
    {
        requiredIndexDict.Clear();
        requiredIndexDict.Add(ChapterType.Chapter1, chapter1List);
        requiredIndexDict.Add(ChapterType.Chapter2, chapter2List);
        requiredIndexDict.Add(ChapterType.Chapter3, chapter3List);
        requiredIndexDict.Add(ChapterType.Chapter4, chapter4List);
        requiredIndexDict.Add(ChapterType.Chapter5, chapter5List);
    }
}

[Serializable]
public class RequiredIndex
{
    public CharacterType characterType;
    public int progress;
    public int index;
}