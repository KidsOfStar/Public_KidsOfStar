using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    public float scrollSpeed = 5f;
    
    private void Update()
    {
        transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
    }
}