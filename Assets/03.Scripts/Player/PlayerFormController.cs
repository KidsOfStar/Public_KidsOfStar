using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPusher
{
    float GetPushPower();
}

public class PlayerFormController : MonoBehaviour, IWeightable, IPusher
{
    [SerializeField, Tooltip("형태변환 데이터 모음집")] private PlayerFormData formData;
    [SerializeField, Tooltip("이펙트 재생용 오브젝트")] private GameObject formChangeEffectObj;
    [SerializeField] private SpriteRenderer spriteRenderer;
    // 플레이어 전체 형태 데이터 딕셔너리
    private Dictionary<PlayerFormType, FormData> formDataDictionary = 
        new Dictionary<PlayerFormType, FormData>();

    private PlayerController controller;    // 플레이어 컨트롤러
    private CapsuleCollider2D capsuleCollider;
    private FormData curFormData;   // 현재 형태의 데이터
    public FormData CurFormData { get { return  curFormData; } }

    // 변신 이펙트 재생 시간
    private float fxDuration;

    // Hide 스킬 활성화 여부
    public bool isInHideArea = false;


    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="player">플레이어 스크립트</param>
    public void Init(Player player)
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        SetFormData();
        controller = player.Controller;
        Animator fxAnim = formChangeEffectObj.GetComponent<Animator>();
        fxDuration = fxAnim.runtimeAnimatorController.animationClips[0].length;
    }

    /// <summary>
    /// 형태 데이터를 딕셔너리에 넣어주는 함수
    /// 키는 형태의 이름으로
    /// </summary>
    void SetFormData()
    {
        for(int i = 0; i < formData.PlayerFromDataList.Count; i++)
        {
            formDataDictionary.Add(formData.PlayerFromDataList[i].playerFormType, formData.PlayerFromDataList[i]);
        }
    }

    /// <summary>
    /// 게임 플레이용 형태 변화
    /// 형태 변화가 가능한 상태인지를 판별
    /// </summary>
    /// <param name="formName">변하려는 형태의 이름</param>
    public void FormChange(PlayerFormType formType)
    {
        // 매개 변수로 온 형태 이름으로 변하려는 형태 데이터 찾기
        FormData nextFormData = formDataDictionary[formType];
        
        // 변할 수 있는 상태인지 체크
        // 요청된 형태가 게임상 존재하는 형태인지 || 요청된 형태가 해금된 상태인지 || 플레이어 조작이 가능한 상태인지
        // || 현재 형태 데이터가 비어있는 상태가 아니고(초기화 전에는 비어있으므로 예외를 위함) 플레이어가 땅에 닿은 상태인지
        if (nextFormData == null || !controller.IsControllable 
            || (curFormData != null && !controller.IsGround)) return;

        // 현재 형태의 데이터가 비어있다면
        if (curFormData == null)
        {
            // 현재 형태 데이터에 변화하려는 형태 데이터를 적용
            curFormData = nextFormData;
        }
        else
        {
            // 초기화가 끝나서 현재 형태 데이터가 존재하는 경우

            // 이미 변화한 형태로 재용청이 왔다면
            if (formType == curFormData.playerFormType)
            {
                // 인간의 형태로 형태 변화
                curFormData = formDataDictionary[PlayerFormType.Human];
                Managers.Instance.GameManager.SetCurrentForm(PlayerFormType.Human);
            }
            else
            {
                // 현재 형태와 요청된 형태가 다르다면 요청된 형태로 형태 변화
                curFormData = nextFormData;
                Managers.Instance.GameManager.SetCurrentForm(formType);
            }
        }
        
        // 형태 변화 이펙트와 데이터 교체 시작
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.FormChange);
        StartCoroutine(FormChangeSequence());
    }

    /// <summary>
    /// 이펙트 없는 형태 변화
    /// 플레이어가 아닌 게임 진행을 위한 형태 변화
    /// 조건 체크 없이 바로 변화
    /// </summary>
    /// <param name="formType">변하려는 형태의 이름</param>
    public void CutSceneFormChange(PlayerFormType formType)
    {
        FormData nextFormData = formDataDictionary[formType];

        // 요청된 형태가 존재하는 형태가 아니라면 로그 띄우기
        if(nextFormData == null)
        {
            EditorLog.LogWarning("존재하지 않는 형태 변화 시도");
            return;
        }

        Managers.Instance.GameManager.SetCurrentForm(formType);
        
        curFormData = nextFormData;
        spriteRenderer.sprite = curFormData.FormImage;
        capsuleCollider.direction = curFormData.ColliderDirection;
        capsuleCollider.offset = new Vector2(curFormData.OffsetX, curFormData.OffsetY);
        capsuleCollider.size = new Vector2(curFormData.SizeX, curFormData.SizeY);
        controller.JumpForce = curFormData.JumpForce;
        controller.Anim.runtimeAnimatorController = curFormData.FormAnim;
    }

    /// <summary>
    /// 형태 변화 이펙트 및 실제 데이터 교체
    /// </summary>
    /// <returns></returns>
    private IEnumerator FormChangeSequence()
    {
        // 플레이어 조작 잠금
        controller.LockPlayer();
        // 일시적으로 플레이어 캐릭터의 스프라이트 렌더러 비활성화
        spriteRenderer.enabled = false;

        // 이펙트 재생용 자식 오브젝트 활성화
        formChangeEffectObj.SetActive(true);
        // 애니메이션 재생 상태 확보를 위한 한 프레임 대기
        yield return null;
        // 이펙트 재생 시간만큼 대기
        yield return new WaitForSeconds(fxDuration);

        // 이펙트 재생용 오브젝트 비활성화
        formChangeEffectObj.SetActive(false);
        // 현재 형태에 맞춰 데이터 교체
        controller.IsControllable = true;
        spriteRenderer.sprite = curFormData.FormImage;
        capsuleCollider.direction = curFormData.ColliderDirection;
        capsuleCollider.offset = new Vector2(curFormData.OffsetX, curFormData.OffsetY);
        capsuleCollider.size = new Vector2(curFormData.SizeX, curFormData.SizeY);
        controller.JumpForce = curFormData.JumpForce;
        controller.Anim.runtimeAnimatorController = curFormData.FormAnim;
        // 플레이어 스프라이트 렌더러 활성화
        spriteRenderer.enabled = true;
    }

    /// <summary>
    /// 플레이어 스프라이트 렌더러 플립
    /// </summary>
    /// <param name="dir">현재 플레이어의 이동 방향</param>
    public void FlipControl(Vector2 dir)
    {
        // 플레이어 조작이 잠금 상태라면 return
        if (!controller.IsControllable) return;

        // 입력이 없는 상태가 아니라면
        if (dir != Vector2.zero)
        {
            if (curFormData.Direction == DefaultDirection.Right)
            {
                spriteRenderer.flipX = dir.x < 0;
            }
            else
            {
                spriteRenderer.flipX = dir.x > 0;
            }
        }
    }

    // 현재 형태의 이름을 반환
    public PlayerFormType ReturnCurFormType()
    {
        return curFormData.playerFormType;
    }

    public float GetWeight()
    {
        return curFormData.Weight;
    }

    public float GetPushPower()
    {
        return curFormData.Force;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 은신지역에 들어갈 때
        if (collision.CompareTag("HideArea"))
        {
            // 대화 시작
            isInHideArea = true;
        }
        else if(collision.CompareTag("Ladder"))
        {
            // 사다리에 닿았을 때
            controller.TouchLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 은신지역에서 벗어날 때
        if (collision.CompareTag("HideArea"))
        {
            // 대화 시작
            isInHideArea = false;
        }
        else if(collision.CompareTag("Ladder"))
        {
            // 사다리에서 떨어졌을 때
            controller.TouchLadder = false;
        }
    }
}