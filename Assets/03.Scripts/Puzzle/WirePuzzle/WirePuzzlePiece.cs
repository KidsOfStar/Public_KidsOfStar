using UnityEngine;
using UnityEngine.UI;

public enum WireColorType
{
    Yellow,
    Blue,
    Red,
    Green
}

public class WirePuzzlePiece : MonoBehaviour
{
    [SerializeField, Tooltip("퍼즐 조각의 이미지 컴포넌트")] private Image puzzlePieceImage;
    [SerializeField, Tooltip("배선 색깔")] private WireColorType wireColer;
    public WireColorType WireColor
    {
        get {  return wireColer; }
        set { wireColer = value; }
    }
    
    // 배열 좌표
    private Vector2Int gridPosition;
    public Vector2Int GridPosition {  get { return gridPosition; } }

    /// <summary>
    /// 퍼즐 조각 초기화
    /// </summary>
    /// <param name="x">배열 x 좌표</param>
    /// <param name="y">배열 y 좌표</param>
    /// <param name="sprite">퍼즐 조각에 적용될 스프라이트</param>
    public void InitPiece(int x, int y, Sprite sprite)
    {
        // 좌표 적용
        gridPosition = new Vector2Int(x, y);
        SetSprite(sprite);
    }

    /// <summary>
    /// 퍼즐 조각에 스프라이트 적용
    /// </summary>
    /// <param name="sprite">적용할 스프라이트</param>
    public void SetSprite(Sprite sprite)
    {
        puzzlePieceImage.sprite = sprite;
    }

    /// <summary>
    /// 현재 스프라이트 반환
    /// </summary>
    /// <returns>현재 스프라이트</returns>
    public Sprite GetSprite()
    {
        return puzzlePieceImage.sprite;
    }
}