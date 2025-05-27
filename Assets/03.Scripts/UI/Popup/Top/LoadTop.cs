using UnityEngine;
using UnityEngine.UI;

public class LoadTop : PopupBase
{
    private int slotIndex {  get; set; }
    public Button checkButton;

    public void SetUp(int index)
    {
        slotIndex = index;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        checkButton.onClick.AddListener(OnCheck);

    }
    private void Load()
    {
        Managers.Instance.SaveManager.Load(slotIndex);
    }
    
    private void OnCheck()
    {
        // 씬 불러오기
        Load();
        Time.timeScale = 1;
        var loadScene = Managers.Instance.GameManager.CurrentScene;
        Managers.Instance.SceneLoadManager.LoadScene(loadScene);
    }
}
