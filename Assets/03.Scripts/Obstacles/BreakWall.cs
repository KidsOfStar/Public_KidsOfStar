using UnityEngine;

public class BreakWall : MonoBehaviour
{
    // 벽을 부술 수 있는 오브젝트
    // 벽을 부술 수 있는 오브젝트는 Player의 형태변화 강아지

    public Collider2D wallCollider;

    public Player player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerFormController formController))
        {
            if (formController.CurFormData.playerFormType == PlayerFormType.Dog)
            {
                Destroy(gameObject);
                Managers.Instance.SoundManager.PlaySfx(SfxSoundType.WallBreak);
            }
        }
    }
}

