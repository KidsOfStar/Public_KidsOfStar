using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/TreePuzzleData")]
public class TreePuzzleData : ScriptableObject
{
    public string puzzleId;
    public Sprite backgroundSprite;     
    public List<Sprite> pieceSprites;
    public int gridWidth = 4;
    public float easyTimeLimit = 90f;
    public float hardTimeLimit = 90f;
}
