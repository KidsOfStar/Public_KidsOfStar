using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool followTarget = true;
    [SerializeField] private Vector3 offset = new(0, 2f, -10f);
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private float smoothTime = 0.15f;
    
    [Header("Background")] // 씬에 배치 할 배경 오브젝트 : 컷씬 재생 중에는 비활성화
    [SerializeField] private GameObject background;
    
    private Transform target = null;
    private CutSceneManager cutSceneManager;
    private Vector3 velocity = Vector3.zero;
    
    private const float SmoothSpeed = 8f;

    public void Init()
    {
        cutSceneManager = Managers.Instance.CutSceneManager;
        target = Managers.Instance.GameManager.Player.transform;

        if (background)
        {
            cutSceneManager.OnCutSceneStart += InactivateBackground;
            cutSceneManager.OnCutSceneEnd += ActiveBackground;
        }
    }

    private void FixedUpdate()
    {
        if (!followTarget)
            return;
        
#if UNITY_EDITOR
        if (Managers.Instance.IsDebugMode && !target)
        {
            target = FindObjectOfType<Player>().transform;
        }
#endif
        if (!target || cutSceneManager.IsCutScenePlaying) return;
        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);
        transform.position = smoothedPosition;
    }
    
    private void ActiveBackground()
    {
        if (!background) return;
        background.SetActive(true);
    }
    
    private void InactivateBackground()
    {
        if (!background) return;
        background.SetActive(false);
    }
    
    private void OnDestroy()
    {
        cutSceneManager.OnCutSceneStart -= InactivateBackground;
        cutSceneManager.OnCutSceneEnd -= ActiveBackground;
    }
}