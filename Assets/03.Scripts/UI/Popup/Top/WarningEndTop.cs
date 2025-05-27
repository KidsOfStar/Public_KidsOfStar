using UnityEngine.UI;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class WarningEndTop : PopupBase
{
    public Button allowBtn;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Time.timeScale = 0; // 게임 일시 정지

        allowBtn.onClick.AddListener(OnExitBtnClick);
    }
    private void OnExitBtnClick()
    {
        Time.timeScale = 1; //게임 시작

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
        //Managers.Instance.SceneLoadManager.LoadScene(SceneType.Title);
        //OnClose();
#elif UNITY_ANDROID
        // 게임 종료
        Application.Quit(); // 빌드된 게임에서 종료
#elif UNITY_WEBGL
        Managers.Instance.SceneLoadManager.LoadScene(SceneType.Title);
        HideDirect();
#endif
    }
    //public override void HideDirect()
    //{
    //    gameObject.SetActive(false);
    //}

}
