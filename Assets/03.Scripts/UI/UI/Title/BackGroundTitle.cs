public class BackGroundTitle : UIBase
{
    // Start is called before the first frame update
    void Start()
    {
        // 현재 경로 
        Managers.Instance.UIManager.Show<UITitle>();
    }
}