using UnityEngine;

public class CutSceneLoad : MonoBehaviour
{
    public Collider2D Collider; // 트리거 콜라이더
    public CutSceneType cutSceneType;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Managers.Instance.CutSceneManager.PlayCutScene(cutSceneType);

            if (cutSceneType == CutSceneType.MeetingBihyi)
            {
                var bgm = Managers.Instance.PoolManager.Spawn<BgmLayeredFader>("MainBgmFader");
                bgm.Init();
            }

            Collider.enabled = false; // 트리거가 한 번만 작동하도록 비활성화
        }
    }
}