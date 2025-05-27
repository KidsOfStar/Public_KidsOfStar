using System;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [Header("UI 위치 설정")]
    public UIPosition uiPosition = UIPosition.UI;

    // 닫힐 때 호출될 콜백
    public Action<object[]> closed;

    /// <summary>
    /// UI가 열릴 때 호출되는 함수
    /// </summary>
    /// <param name="param"></param>
    public virtual void Opened(params object[] param) { }

    /// <summary>
    /// UI를 즉시 비활성화할 때 호출되는 함수
    /// </summary>
    public virtual void HideDirect()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// UI 활성화/비활성화
    /// </summary>
    /// <param name="isActive"></param>
    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
