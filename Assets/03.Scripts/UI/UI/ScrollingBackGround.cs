using UnityEngine;

public class ScrollingBackGround : MonoBehaviour
{
    private Transform cam; //메인카메라 
    private Vector3 camStartPos;
    private float distance;

    private GameObject[] backGrounds;
    private Material[] mat;
    private float[] backSpeed;

    private float farthestBack;

    [Range(0f, 0.5f)]
    public float parallaxSpeed = 0.2f;

    private Vector2 offset = Vector2.zero;
    private void Start()
    {
        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backGrounds = new GameObject[backCount];

        for(int i = 0; i< backCount; i++)
        {
            backGrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backGrounds[i].GetComponent<Renderer>().sharedMaterial;
        }
        BackSpeedCalculate(backCount);
    }
    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            if ((backGrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backGrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
           backSpeed[i] = 1 - (backGrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void FixedUpdate()
    {
        if (cam == null) return;

        distance = cam.position.x - camStartPos.x;
        transform.position = new Vector3(cam.position.x, transform.position.y, 0);

        for (int i = 0; i < backGrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            // mat[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);

            offset.x = distance * speed;
            offset.y = 0;
            mat[i].SetTextureOffset("_MainTex", offset);
        }
    }

    public void Initialized(Transform cameraTransform)
    {
        cam = cameraTransform;
        camStartPos = cam.position;
    }
}
