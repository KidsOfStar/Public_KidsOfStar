using System.Collections.Generic;
using UnityEngine;

// 형태 스프라이트 기본 방향
public enum DefaultDirection
{
    Right,
    Left
}

[System.Serializable]
public class FormData
{
    [SerializeField, Tooltip("형태 이름")] private string formName;
    public string FormName { get { return formName; } }
    [SerializeField, Tooltip("형태의 스프라이트")] private Sprite formImage;
    public Sprite FormImage { get { return formImage; } }
    // 애니메이터 오버라이드 컨트롤러
    [SerializeField, Tooltip("애니메이션 클립 교체용")] private AnimatorOverrideController formAnim;
    public AnimatorOverrideController FormAnim { get { return formAnim; } }
    [SerializeField, Tooltip("형태의 점프력")] private float jumpForce;
    public float JumpForce { get { return jumpForce; } }

    // 콜라이더 기본 방향
    [SerializeField] private CapsuleDirection2D colliderDirection;
    public CapsuleDirection2D ColliderDirection { get { return colliderDirection; } }

    // 형태의 콜라이더 offset
    [SerializeField] private float offsetX;
    public float OffsetX { get { return offsetX; } }
    [SerializeField] private float offsetY;
    public float OffsetY { get { return offsetY; } }
    // 형태의 콜라이더 크기
    [SerializeField] private float sizeX;
    public float SizeX { get { return sizeX; } }
    [SerializeField] private float sizeY;
    public float SizeY { get { return sizeY; } }
    [SerializeField, Tooltip("형태의 미는 힘")] private float force;
    public float Force { get { return force; } }
    [SerializeField, Tooltip("형태의 무게")] private float weight;
    public float Weight { get { return weight; } }
    // 해금 상태
    public bool IsActive = false;
    [SerializeField, Tooltip("스프라이트가 기본으로 바라보는 방향")] private DefaultDirection direction;
    public DefaultDirection Direction { get { return direction; } }

    public PlayerFormType playerFormType;
}

[CreateAssetMenu(fileName = "new PlayerForm Data", menuName = "PlayerForm Data")]
public class PlayerFormData : ScriptableObject
{
    [SerializeField, Tooltip("형태 데이터 리스트")] private List<FormData> playerFromDataList = new List<FormData>();
    public List<FormData> PlayerFromDataList { get { return playerFromDataList; } }
}