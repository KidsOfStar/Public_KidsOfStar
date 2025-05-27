using UnityEngine;

public class LoadSceneDebug : MonoBehaviour
{
#if TEST_BUILD || UNITY_EDITOR
    [SerializeField] private float buttonWidth = 180f;
    [SerializeField] private float buttonHeight = 60f;
    [SerializeField] private int fontSize = 30;
    private GUIStyle buttonStyle;
    private bool isDebugMode = false;

    private void Start()
    {
        buttonStyle = new GUIStyle();
        buttonStyle.fontSize = fontSize;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.hover.textColor = Color.black;

        Texture2D backgroundTexture = new(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0.3f, 0.6f, 0.7f, 0.5f));
        backgroundTexture.Apply();

        buttonStyle.normal.background = backgroundTexture;
        buttonStyle.hover.background = backgroundTexture;  // 호버 배경도 설정
        buttonStyle.active.background = backgroundTexture; // 클릭시 배경도 설정

        buttonStyle.fixedWidth = buttonWidth;
        buttonStyle.fixedHeight = buttonHeight;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            isDebugMode = !isDebugMode;
        }
    }

    private void OnGUI()        
    {
        if (!isDebugMode) return;
        
        float startX = Screen.width - buttonWidth - 10f;
        float startY = Screen.height / 3f;

        Rect buttonRect = new(startX, startY, buttonWidth, buttonHeight);
        if (GUI.Button(buttonRect, "Time", buttonStyle))
        {
            EditorLog.Log(Time.timeScale);
        }

        buttonRect.y += buttonHeight + 10f;
        if (GUI.Button(buttonRect, "Chase", buttonStyle))
        {
            Managers.Instance.SceneLoadManager.LoadScene(SceneType.Chapter1Puzzle);
        }

        buttonRect.y += buttonHeight + 10f;
        if (GUI.Button(buttonRect, "Chapter 02", buttonStyle))
        {
            Managers.Instance.SceneLoadManager.LoadScene(SceneType.Chapter2);
        }

        buttonRect.y += buttonHeight + 10f;
        if (GUI.Button(buttonRect, "Chapter 03", buttonStyle))
        {
            Managers.Instance.SceneLoadManager.LoadScene(SceneType.Chapter3);
        }
    }
#endif
}