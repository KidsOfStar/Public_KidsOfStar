using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreePuzzlePiece : MonoBehaviour, IPointerClickHandler
{
    private TreePuzzleSystem system;
    private int curIndex;

    [SerializeField] private Image pieceImage; // UI용 조각 이미지
    [SerializeField] private int correctRotation; // 0, 90, 180, 270

    private int currentRotation;
    [SerializeField] private GameObject outLine;

    private void Awake()
    {
        outLine.SetActive(false);
        pieceImage.raycastTarget = true;
    }
    public void Initialize(TreePuzzleSystem systemManager, int correctionRotation, int index)
    {
        system = systemManager;
        correctRotation = correctionRotation;
        currentRotation = 0;
        this.curIndex = index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!system.IsRunning) return;

        // 같은 조각을 다시 클릭하면 회전
        if (system.SelectedIndex == curIndex)
        {
            RotateRight();
        }
        // 다른 조각 클릭 시, 단순 선택 변경
        else
        {
            system.OnPieceSelected(curIndex);
        }
    }

    public void RotateRight()
    {
        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.TurnPuzzle);

        currentRotation = (currentRotation + 90) % 360;
        pieceImage.rectTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);

        system.CheckPuzzle();
    }

    public void RandomizeRotation()
    {
        currentRotation = 90 * Random.Range(1, 4);
        pieceImage.rectTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);
    }

    public bool IsCorrect()
    {
        return currentRotation == correctRotation;
    }

    public void SetSprite(Sprite sprite)
    {
        pieceImage.sprite = sprite;
    }

    public void SetHighlight(bool on)
    {
        if (outLine != null)
            outLine.SetActive(on);
    }
}


