using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : ISceneLifecycleHandler
{
    public RectTransform CanvasRectTr { get; private set; }
    private List<Transform> parents;

    private Dictionary<string, UIBase>
        uiList = new Dictionary<string, UIBase>(); // UI 리스트를 Dictionary로 변경하여 이름으로 접근 가능하게 함

    private Dictionary<string, List<UIBase>>
        multiListUIList = new Dictionary<string, List<UIBase>>(); // 여러 개의 UI를 관리하기 위한 리스트


    /// <summary>
    /// UI를 생성할 부모 오브젝트 리스트를 설정
    /// 보통 Canvas 하위에 Background, UI, Popup 같은 위치들이 있음
    /// </summary>
    public void SetParents(List<Transform> parents)
    {
        this.parents = parents;
        uiList.Clear(); // 기존 UI 리스트 초기화 (중복 방지 및 초기화 목적)
    }

    /// <summary>
    /// UI를 보여줌. 없으면 생성하고, 있으면 기존 UI를 재활용함.
    /// param: UI가 열릴 때 필요한 매개변수 전달
    /// </summary>
    public T Show<T>(params object[] param) where T : UIBase
    {
        string uiName = typeof(T).Name; // 제네릭 타입의 이름으로 UI 이름 결정

        // 이미 생성되어 있는 UI가 있는지 확인
        var uiDictionary = uiList.TryGetValue(uiName, out var ui);

        // 없으면 Resource에서 로드하여 생성
        if (!uiDictionary)
        {
            // ResourceManager를 통해 UI 프리팹 로드
            var prefab = Managers.Instance.ResourceManager.Load<T>(GetPath(uiName), false);

            if (prefab == null) return null; // 프리팹이 없으면 null 반환

            ui = Object.Instantiate(prefab, parents[(int)prefab.uiPosition]) as T; // 지정된 위치에 생성
            ui.name = uiName;
            uiList.Add(uiName, ui); // 생성된 UI를 Dictionary에 추가
        }

        ui.SetActive(true);
        ui.Opened(param); // UI가 열릴 때 호출되는 함수
        return (T)ui;     // UIBase로 캐스팅하여 반환
    }

    private string GetPath(string name)
    {
        /// <summary>
        /// 주어진 이름에 따라 경로를 설정합니다
        /// Canvas, Popup, Top에 따라 경로를 반환
        /// 조건에 모두 해당하지 않을 경우 Define.UIPath를 사용하여 반환
        /// </summary>
        if (name.Contains("Canvas"))
        {
            return Define.uiPath + name;
        }
        else if (name.Contains("BackGround"))
        {
            return Define.uiBG + name;
        }
        else if (name.Contains("Popup"))
        {
            return Define.popupPath + name;
        }
        else if (name.Contains("Top"))
        {
            return Define.topPath + name;
        }
        else
        {
            return Define.uiPath + name;
        }
    }

    /// <summary>
    /// UI를 숨기기
    /// </summary>
    public void Hide<T>(params object[] param) where T : UIBase
    {
        string uiName = typeof(T).Name;
        var uiDictionary = uiList.TryGetValue(uiName, out var ui);

        if (uiDictionary)
        {
            ui.closed?.Invoke(param);
            ui.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 특정 UI를 가져옴 (없으면 null)
    /// </summary>
    public T Get<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;
        return uiList.TryGetValue(uiName, out var ui) ? (T)ui : null;
    }

    /// <summary>
    /// 특정 UI가 열려있는지 확인
    /// </summary>
    public bool IsOpened<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;
        return uiList.TryGetValue(uiName, out var ui) && ui.gameObject.activeInHierarchy;
    }

    public void OnSceneLoaded() // 씬 로드할 때마다
    {
        var currentScene = Managers.Instance.SceneLoadManager.CurrentScene;
        var canvasPrefab = Managers.Instance.ResourceManager.Load<Canvas>("UI/Canvas", true);

        if (canvasPrefab == null)
        {
            EditorLog.LogError("Canvas prefab not found at path: UI/Canvas");
            return;
        }

        // Canvas 인스턴스 생성
        var canvasInstance = Object.Instantiate(canvasPrefab);
        canvasInstance.name = "Canvas";
        CanvasRectTr = canvasInstance.transform as RectTransform;

        List<Transform> parentList = new List<Transform>();

        // Canvas 하위에 UI, Popup, Top 위치 생성
        string[] childNames = {"BG", "UI", "Popup", "Top" };

        foreach (string childName in childNames)
        {
            var child = canvasInstance.transform.Find(childName);
            if (child != null)
            {
                parentList.Add(child);
            }
            else
            {
                EditorLog.LogWarning($"Canvas 하위에서 '{childName}' 오브젝트를 찾지 못했습니다.");
            }
        }

        // 활성화 된 씬에 배치
#if UNITY_EDITOR
        if (Managers.Instance.IsDebugMode)
        {
            SetParents(parentList);
            return;
        }
#endif
        
        var activeScene = SceneManager.GetSceneByName(currentScene.GetName());
        SceneManager.MoveGameObjectToScene(canvasInstance.gameObject, activeScene);

        // 생성된 parentList를 SetParents()에 전달하여 부모 목록을 설정
        SetParents(parentList);
    }

    public void OnSceneUnloaded() { }
}