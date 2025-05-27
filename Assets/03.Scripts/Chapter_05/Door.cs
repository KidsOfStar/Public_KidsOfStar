using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject bubbleTextPrefab;
    private GameObject bubbleTextInstance; // 문 위에 생성된 프리팹 인스턴스

    // 이동할 씬 설정하기
    public SceneType sceneType;

    // 문 잠금 여부
    public bool isDoorOpen = false;
    private SkillBTN skillBTN;
    // 문과 닿을 시 상호작용 키 활성화

    private void Start()
    {
        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;
    }
    //private void OnDestroy()
    //{
    //    if (skillBTN != null)
    //    {
    //        skillBTN.OnInteractBtnClick -= OnInteraction; // 상호작용 버튼 클릭 이벤트 해제
    //    }
    //}

    private void OnInteraction()
    {
        if (!isDoorOpen)
        {
            if (bubbleTextInstance == null && bubbleTextPrefab != null)
            {
                bubbleTextInstance = Instantiate(bubbleTextPrefab, transform);
                bubbleTextInstance.transform.localPosition = new Vector3(0, 2f, 0); // 문 위 위치

                var bubbleText = bubbleTextInstance.GetComponentInChildren<DoorPopup>();
                if (bubbleText != null)
                {
                    bubbleText.Opened(); // 문구 출력
                }
            }
            else if (bubbleTextInstance != null)
            {
                Destroy(bubbleTextInstance);
                bubbleTextInstance = null;
            }
        }
        else
        {
            Managers.Instance.SceneLoadManager.LoadScene(sceneType);
        }
    }

    // 문과 닿을 시 상호작용 키 활성화
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(true); // 상호작용 버튼 활성화
            skillBTN.OnInteractBtnClick += OnInteraction;
        }
    }

    // 문과 떨어질 시 상호작용 키 비활성화
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(false); // 상호작용 버튼 비활성화
            skillBTN.OnInteractBtnClick -= OnInteraction; // 상호작용 버튼 클릭 이벤트 해제

            if (bubbleTextInstance != null)
            {
                Destroy(bubbleTextInstance);
                bubbleTextInstance = null;
            }
        }
    }
}

