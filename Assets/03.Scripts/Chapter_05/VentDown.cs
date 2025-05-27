using System.Collections.Generic;
using UnityEngine;

public class VentDown : MonoBehaviour
{
    [Header("Vent In")]
    public GameObject ventBG;
    public GameObject ventMap;
    public GameObject ventBlockMap;
    public GameObject ventObject;

    [Header("Vent Out")]
    public GameObject timeMap;
    public GameObject Hide;

    private bool isVentActive = false; // ventIn이 현재 활성화 상태인지 여부

    List<GameObject> ventIn = new List<GameObject>();
    List<GameObject> ventOut = new List<GameObject>();

    private SkillBTN skillBTN;

    // Start is called before the first frame update
    void Start()
    {
        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel;

        VentIn();
        VentOut();
        SetActiveGroup(ventIn, false);  // 시작 시 벤트 안 비활성화
        SetActiveGroup(ventOut, true);  // 시작 시 벤트 밖 활성화
    }

    private void OnDestroy()
    {
        skillBTN.OnInteractBtnClick -= OnVentInteraction; // 상호작용 버튼 클릭 이벤트 해제
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Managers.Instance.GameManager.ChapterProgress == 4)
        {
            if (other.CompareTag("Player"))
            {
                skillBTN.ShowInteractionButton(true); // 상호작용 버튼 활성화
                skillBTN.OnInteractBtnClick += OnVentInteraction; // 상호작용 버튼 클릭 이벤트 등록

                if (isVentActive)
                {
                    SetActiveGroup(ventIn, false);  // 벤트 안 비활성화
                    SetActiveGroup(ventOut, true);  // 벤트 밖 활성화
                    isVentActive = false;           // 벤트 밖 상태로 변경
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(false); // 상호작용 버튼 비활성화
            skillBTN.OnInteractBtnClick -= OnVentInteraction; // 이벤트 해제
        }
    }

    private void SetActiveGroup(List<GameObject> objects, bool isActive)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(isActive); // null이 아닌 오브젝트만 활성/비활성 처리
        }
    }

    private void VentIn()
    {
        ventIn.Add(ventBG);
        ventIn.Add(ventMap);
        ventIn.Add(ventBlockMap);
        ventIn.Add(ventObject);
    }

    private void VentOut()
    {
        ventOut.Add(timeMap);
        ventOut.Add(Hide);
    }

    private void OnVentInteraction()
    {
        if (!isVentActive)
        {
            SetActiveGroup(ventIn, true);   // 벤트 안 활성화
            SetActiveGroup(ventOut, false); // 벤트 밖 비활성화
            isVentActive = true;            // 벤트 안 상태로 변경
        }
    }
}
