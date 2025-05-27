using UnityEngine;

public class CutSceneNpc : MonoBehaviour, IDialogSpeaker
{
    [SerializeField] private CharacterType characterType;
    [SerializeField] private Transform bubbleTr;
    
    public CharacterType GetCharacterType()
    {
        return characterType;
    }
    
    public Transform GetBubbleTr()
    {
        return bubbleTr;
    }
}
