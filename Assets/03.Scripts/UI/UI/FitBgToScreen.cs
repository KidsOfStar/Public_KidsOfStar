using UnityEngine;

public class FitBgToScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FitToScreen();
    }

    void FitToScreen()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // 월드 기준 카메라 크기 계산
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        // 현재 스프라이트의 크기
        Vector2 spriteSize = sr.sprite.bounds.size;

        // 비율 계산 (꽉 채우기 위해)
        float scaleFactor = Mathf.Max(worldScreenWidth / spriteSize.x, worldScreenHeight / spriteSize.y);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

    }
}
