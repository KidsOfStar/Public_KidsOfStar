using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 컷씬용 다이얼로그 플레이어
public class DialogPlayer : MonoBehaviour
{
    [field: SerializeField] private SignalAsset dialogSignal;
    [field: SerializeField] private int[] dialogIndexes;
    [field: SerializeField] public CutSceneNpc[] Npcs { get; private set; }
    private int currentIndex;

    public void ShowDialog(PlayableDirector director)
    {
        if (dialogIndexes.Length == 0)
        {
            EditorLog.LogError("DialogPlayer: dialogIndex is empty");
            return;
        }
        
        if (currentIndex >= dialogIndexes.Length)
        {
            EditorLog.LogWarning("DialogPlayer: All dialogs are played");
            return;
        }
        
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);

        var index = dialogIndexes[currentIndex];
        Managers.Instance.DialogueManager.SetCurrentDialogData(index);
        currentIndex++;
    }
}