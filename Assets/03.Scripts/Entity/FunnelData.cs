using System;
using System.Collections.Generic;

[Serializable]
public class FunnelData
{
    public DialogPair[] dialogStartFunnel;
    public DialogPair[] dialogEndFunnel;
    public CutScenePair[] cutSceneStartFunnel;

    public Dictionary<int, int> GetDialogStartDict()
    {
        Dictionary<int, int> dialogStartDict = new();
        foreach (var pair in dialogStartFunnel)
        {
            dialogStartDict.Add(pair.dialogIndex, pair.funnelNum);
        }

        return dialogStartDict;
    }
    
    public Dictionary<int, int> GetDialogEndDict()
    {
        Dictionary<int, int> dialogEndDict = new();
        foreach (var pair in dialogEndFunnel)
        {
            dialogEndDict.Add(pair.dialogIndex, pair.funnelNum);
        }

        return dialogEndDict;
    }
    
    public Dictionary<string, int> GetCutSceneStartDict()
    {
        Dictionary<string, int> cutSceneStartDict = new();
        foreach (var pair in cutSceneStartFunnel)
        {
            cutSceneStartDict.Add(pair.cutSceneIndex, pair.funnelNum);
        }

        return cutSceneStartDict;
    }
}

[Serializable]
public struct DialogPair
{
    public int dialogIndex;
    public int funnelNum;
}

[Serializable]
public struct CutScenePair
{
    public string cutSceneIndex;
    public int funnelNum;
}