using UnityEngine.UI;

public class WarningStartTop : PopupBase
{
    public Button appltBtn;
    protected override void Start()
    {
        base.Start();
        appltBtn.onClick.AddListener(NextSence);
    }

    public override void HideDirect()
    {
        gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        HideDirect();
    }

    public void NextSence()
    {
        Managers.Instance.SceneLoadManager.LoadScene(SceneType.Chapter1);
    }
}
