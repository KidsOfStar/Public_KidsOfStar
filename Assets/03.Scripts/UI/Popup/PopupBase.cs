using UnityEngine;
using UnityEngine.UI;

public class PopupBase : UIBase
{
    [SerializeField] protected Button closeBtn;
    [Tooltip("닫을 때 Time.timeScale = 1로 복구할지 여부")]
    public bool checkTimeStop = false;
    public bool isFirst = false;


    public override void Opened(params object[] param)
    {
        if(checkTimeStop) 
            Time.timeScale = 0; // 게임 일시 정지
        // 특정 bool를 이용해서 앞에서 보여지게 하기
        if(isFirst)
            transform.SetAsLastSibling();
        if (Managers.Instance.SceneLoadManager.CurrentScene != SceneType.Title)
        {
            Managers.Instance.GameManager.Player.Controller.UnlockPlayer();
        }
    }

    protected virtual void Start()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(() => 
            {
                Managers.Instance.SoundManager.PlaySfx(SfxSoundType.UICancel);
                HideDirect();
            });
        }
    }

    public override void HideDirect()
    {
        base.HideDirect();
        if (checkTimeStop)
        {
            Time.timeScale = 1; // 게임 재개
            if (Managers.Instance.SceneLoadManager.CurrentScene != SceneType.Title)
            {
                Managers.Instance.GameManager.Player.Controller.UnlockPlayer();
            }
        }
    }
}
