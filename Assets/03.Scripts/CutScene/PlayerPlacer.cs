using UnityEngine;

public class PlayerPlacer : MonoBehaviour
{
    [SerializeField] private Vector3 movePosition;

    public void MovePlayer()
    {
        if (movePosition == Vector3.zero)
            EditorLog.LogWarning("PlayerPlacer : movePosition is Vector3.zero");
        
        var player = Managers.Instance.GameManager.Player;
        player.transform.position = movePosition;
    }
}
