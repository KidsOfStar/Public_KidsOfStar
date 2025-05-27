using System.Collections.Generic;
using UnityEngine;


public class VentDoor : MonoBehaviour
{
    [Header("Vent In")]
    public List<GameObject> ventIn = new List<GameObject>();

    [Header("Vent Out")]
    public List<GameObject> ventOut = new List<GameObject>();

    private bool isVentInOut = false; // 벤트 안으로 들어갔는지 여부


    private SkillBTN skillBTN;

    // Start is called before the first frame update
    void Start()
    {
        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(true); // 상호작용 버튼 비활성화
            skillBTN.OnInteractBtnClick += OnVentInteraction; // 상호작용 버튼 클릭 이벤트 등록
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(false); // 상호작용 버튼 비활성화
            skillBTN.OnInteractBtnClick -= OnVentInteraction; // 상호작용 버튼 클릭 이벤트 등록
        }
    }

    private void SetActiveGroup(List<GameObject> objects, bool isActive)
    {
        foreach (GameObject obj in objects)
        {
            // 오브젝트가 null이 아닐 경우에만 활성화/비활성화
            if (obj != null)
                obj.SetActive(isActive);
        }
    }

    private void OnVentInteraction()
    {
        if (!isVentInOut)  
        {
            SetActiveGroup(ventIn, true);
            SetActiveGroup(ventOut, false);
            isVentInOut = true; // 벤트 안으로 들어갔는지 여부
        }
        else
        {
            SetActiveGroup(ventIn, false);
            SetActiveGroup(ventOut, true);
            isVentInOut = false; // 벤트 안으로 들어갔는지 여부
        }
    }
}
