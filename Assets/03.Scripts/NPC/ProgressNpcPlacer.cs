using System;
using UnityEngine;

public class ProgressNpcPlacer : MonoBehaviour
{
    [SerializeField] private bool isDisableInStart;
    [SerializeField] private NpcPositionData[] positionDatas;
    
    public void Init()
    {
        Managers.Instance.GameManager.OnProgressUpdated += MoveNpcToPosition;
        
        if (isDisableInStart) gameObject.SetActive(false);
    }
    
    private void MoveNpcToPosition()
    {
        if (!this)
            Managers.Instance.GameManager.OnProgressUpdated -= MoveNpcToPosition;
        
        for (int i = 0; i < positionDatas.Length; i++)
        {
            NpcPositionData positionData = positionDatas[i];
            if (positionData.progress == Managers.Instance.GameManager.ChapterProgress)
            {
                // NPC를 해당 위치로 이동
                transform.position = positionData.position;
                
                // NPC 활성화 여부에 따라 활성화/비활성화
                gameObject.SetActive(positionData.isActive);
                
                if (gameObject.TryGetComponent(out SpriteRenderer sr))
                    sr.flipX = positionData.isFlip;
            }
        }
    }

    private void OnDestroy()
    {
        Managers.Instance.GameManager.OnProgressUpdated -= MoveNpcToPosition;
    }
}

[Serializable]
public struct NpcPositionData
{
    public int progress;
    public bool isFlip;
    public bool isActive;
    public Vector3 position;
}