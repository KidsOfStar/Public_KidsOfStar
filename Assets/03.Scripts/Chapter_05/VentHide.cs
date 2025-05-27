using UnityEngine;

public class VentHide : MonoBehaviour
{
    public GameObject ventMapEffect; // VentMapEffect

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // VentMapEffect 투명화하기
            ventMapEffect.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // VentMapEffect 다시 보이게 하기
            ventMapEffect.SetActive(true);
        }
    }
}
