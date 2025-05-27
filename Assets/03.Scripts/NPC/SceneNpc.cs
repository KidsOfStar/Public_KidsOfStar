using UnityEngine;
public class SceneNpc : InteractSpeaker, IDialogSpeaker
{
    public CharacterType GetCharacterType()
    {
        return CharacterType;
    }

    public Transform GetBubbleTr()
    {
        return BubbleTr;
    }
}
