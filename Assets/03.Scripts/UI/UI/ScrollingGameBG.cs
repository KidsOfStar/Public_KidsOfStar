using UnityEngine;

public class ScrollingGameBG : MonoBehaviour
{
    [Tooltip("스크롤 속도")]
    public float scrollSpeed;
    private Material mat;
    private Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset.x += scrollSpeed * Time.deltaTime;
        offset.x = Mathf.Repeat(offset.x, 1f);
        mat.mainTextureOffset = offset;
    }
}
