using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadingData", menuName = "ScriptableObject/LoadingData")]
public class LoadingData : ScriptableObject
{
    [Header("Backgrounds")]
    public Sprite[] Backgrounds;
    
    [Header("Tooltips")]
    public Tooltip[] Tooltips;
}

[Serializable]
public struct Tooltip
{
    public SceneType sceneType;
    [TextArea(3, 10)] public string[] tooltips;
}