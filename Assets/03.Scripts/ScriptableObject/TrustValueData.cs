using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaxTrustData", menuName = "ScriptableObject/MaxTrustData")]
public class TrustValueData : ScriptableObject
{
    public TrustData[] trustDatas;

    public Dictionary<ChapterType, TrustData> GetTrustDataDict()
    {
        var dict = new Dictionary<ChapterType, TrustData>();
        for (int i = 0; i < trustDatas.Length; i++)
        {
            var trustData = trustDatas[i];
            dict[trustData.chapter] = trustData;
        }

        return dict;
    }
}

[Serializable]
public class TrustData
{
    public ChapterType chapter;
    public int minTrust;
    public int maxTrust;
    public int endingThreshold;
}