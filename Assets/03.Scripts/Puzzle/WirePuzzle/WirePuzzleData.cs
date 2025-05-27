using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/WirePuzzleData")]
public class WirePuzzleData : TreePuzzleData
{
    [SerializeField, Tooltip("셔플을 위해 랜덤 좌표를 선택할 횟수")] private int shuffleCount;
    public int ShuffleCount { get { return shuffleCount; } }
}
