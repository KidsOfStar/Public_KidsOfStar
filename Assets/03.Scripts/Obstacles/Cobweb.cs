using System.Collections;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    public Collider2D cobwebCollider;
    public float slowDownFacto; // 느려지는 비율
    private float originalJumpForce; // 점프력 원본
    private bool isBreaking = false; // 거미줄 파괴 중인지 여부

    private Coroutine warningTime;
    private bool hasShownWarning = false;
    private void Start()
    {
        cobwebCollider = GetComponent<Collider2D>();
        originalJumpForce = Managers.Instance.GameManager.Player.Controller.JumpForce;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&
            other.TryGetComponent(out PlayerController playerController) &&
            other.TryGetComponent(out PlayerFormController formController))
        {

            // 이동속도 감소
            playerController.MoveSpeed /= slowDownFacto;

            // 점프력 0으로 설정
            playerController.JumpForce = 0f;

            if (formController.CurFormData.playerFormType == PlayerFormType.Dog && !isBreaking)
            {
                StartCoroutine(DogBreakCobweb(playerController));
            }

            if (!hasShownWarning && warningTime == null)
                warningTime = StartCoroutine(ShowCobwebWarningAfterDelay(other.transform));
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")
            && other.TryGetComponent(out PlayerController playerController)
            && other.TryGetComponent(out PlayerFormController formController))
        {
            // 파괴 중이 아니라면 계속 점프력 0으로 유지
            if (!isBreaking && playerController.JumpForce != 0f)
            {
                playerController.JumpForce = 0f;
            }

            if (formController.CurFormData.playerFormType == PlayerFormType.Dog && !isBreaking)
            {
                StartCoroutine(DogBreakCobweb(playerController));
            }
        }
    }

    private IEnumerator DogBreakCobweb(PlayerController playerController)
    {
        isBreaking = true;

        yield return new WaitForSeconds(1f);

        // 점프력 복원
        playerController.JumpForce = originalJumpForce;


        // 거미줄 제거
        Destroy(gameObject);

        Managers.Instance.SoundManager.PlaySfx(SfxSoundType.WallBreak);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        // 점프력은 DogBreakCobweb에서만 복원되도록 수정
        if (other.CompareTag("Player") &&
        other.TryGetComponent(out PlayerController playerController) &&
        other.TryGetComponent(out PlayerFormController formController))
        {
            // 이동속도 복원
            playerController.MoveSpeed *= slowDownFacto;

            // Dog 폼이 아니면 점프력 복원
            if (!isBreaking && formController.CurFormData.playerFormType != PlayerFormType.Dog)
            {
                playerController.JumpForce = originalJumpForce;
            }
        }

        if (warningTime != null)
        {
            StopCoroutine(warningTime);
            warningTime = null;
        }
    }

    private IEnumerator ShowCobwebWarningAfterDelay(Transform playerTransform)
    {
        yield return new WaitForSeconds(1.5f);

        hasShownWarning = true;
        warningTime = null;

        var popup = Managers.Instance.UIManager.Show<WarningPopup>(WarningType.Coweb);

        Vector3 worldAboveHead = playerTransform.position + new Vector3(0, 1.5f, 0);
        popup.SetScreenPosition(worldAboveHead);
    }
}
