using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ConstantWidthCamera : MonoBehaviour
{
    [Header("가로로 보이고 싶은 world 단위 길이")]
    [Tooltip("예: 10이면, 항상 가로 10 유닛이 보이도록 합니다.")]
    public float targetWorldWidth = 18f;

    private Camera cam;
    private int lastScreenWidth, lastScreenHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateOrthographicSize();
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void Update()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            // 화면 크기가 변경되었을 때 orthographicSize를 업데이트
            UpdateOrthographicSize();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    private void UpdateOrthographicSize()
    {
        // 화면 비율(width/height)
        float screenAspect = (float)Screen.width / Screen.height;

        // orthographicSize는 "세로 절반(world unit)"이므로,
        // targetWorldWidth / screenAspect = world 단위 세로 전체 길이
        // 이를 절반으로 잘라서 orthographicSize로 세팅
        cam.orthographicSize = (targetWorldWidth / screenAspect) * 0.5f;
        EditorLog.Log("UpdateOrthographicSize");
    }
}