using UnityEngine;

public class EntranceBlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                var formType = player.FormControl.CurFormData.playerFormType;
                //Debug.Log($"플레이어 형태: {formType}");


                // 은신 폼(Hide)이 아닐 경우 무조건 막기
                if ((formType != PlayerFormType.Hide))
                {
                    // 뒤로 밀어주기
                    player.Controller.BackMove();
                }
            }
        }
    }

    
}
