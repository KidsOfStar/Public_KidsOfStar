using UnityEngine;

// 컷씬용 씬 로더
public class SceneLoadable : MonoBehaviour
{
    [field: SerializeField] public SceneType SceneType { get; private set; }

    public void LoadScene()
    {
        Managers.Instance.SceneLoadManager.LoadScene(SceneType);
    }
}
