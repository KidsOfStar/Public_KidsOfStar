using UnityEngine;

public class DeadIine : MonoBehaviour
{
    public int playerPosX { get; private set; }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                playerPosX = (int)collision.transform.position.x;
                player.gameObject.SetActive(false);
                Managers.Instance.GameManager.TriggerEnding(EndingType.Mistake);
            }
        }
    }
}
