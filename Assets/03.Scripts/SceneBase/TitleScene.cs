using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private BgmLayeredFader bgmLayeredFader;
    protected void Start()
    {
        Managers.Instance.OnSceneLoaded();
        Managers.Instance.SceneLoadManager.IsSceneLoadComplete = true;
        Managers.Instance.UIManager.Show<BackGroundTitle>();
        Managers.Instance.AnalyticsManager.SendFunnel("1");
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.Intro);
    }
    protected void OnDestroy()
    {
        Managers.Instance.SoundManager.StopBgm();
    }
}