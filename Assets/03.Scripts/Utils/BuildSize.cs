using UnityEngine;

public class BuildSize : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_WEBGL //  웹GL 빌드 설정
        int height = 1080;
        int width = (int)(height * (9f / 16f));

        Screen.SetResolution(width, height, false);

#elif UNITY_ANDROID // 안드로이드 빌드 설정
        int height = 1080;
        int width = (int)(height * (9f / 16f));

        Screen.SetResolution(width, height, false);

#elif UNITY_IOS // iOS 빌드 설정
        int height = 1080;
        int width = (int)(height * (9f / 16f));
        Screen.SetResolution(width, height, false);
#endif
    }
}
