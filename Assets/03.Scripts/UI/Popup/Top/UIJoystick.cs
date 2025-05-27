using UnityEngine;
using UnityEngine.EventSystems;

public class UIJoystick : UIBase
{
    [field: SerializeField] public RectTransform joystickBase;
    [field: SerializeField] public RectTransform joystickHandle;
    // [SerializeField, Range(5f, 20f)] private float sensitivity = 5f;

    private Vector2 inputVector = Vector2.zero;
    public Vector2 Direction => inputVector;

    private Vector2 startPos;
    private float maxDistance;

    private void Start()
    {
        startPos = joystickHandle.position;
        maxDistance = joystickBase.sizeDelta.x / 2f;
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.position = startPos;
    }
}