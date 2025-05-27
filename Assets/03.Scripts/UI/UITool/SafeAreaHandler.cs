using UnityEngine;

// RectTransform을 필수로 요구하는 컴포넌트 설정
[RequireComponent(typeof(RectTransform))]
public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;    // RectTransform 컴포넌트

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        // 현재 기기의 안전 영역 (Safe Area) 가져오기
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;  // 최소 사이즈
        Vector2 anchorMax = safeArea.position + safeArea.size;  // 안전 영역의 최대 좌표 = 위치 + 사이즈

        // 비율로 변환 (0~1)
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform의 앵커 설정
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
