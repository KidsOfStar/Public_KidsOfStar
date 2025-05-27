using System;
using UnityEngine;

public class DashInteractable : MonoBehaviour
{
    public InteractionType interactionType; // 상호작용 타입
    public CharacterType npcType; // Jigim 또는 Semyung을 에디터에서 지정

    private SkillBTN skillBTN;
    private DashGame dashGame;

    readonly int teleport = 30006; // 대사 완료 시 플레이어 위치 변경
    readonly int startGame = 30007; // 대사 완료 시 게임 시작
    readonly Vector3 teleportPosition = new Vector3(-7f, 1f, 0); // 예시 위치

    public void Init(DashGame game)
    {
        skillBTN = Managers.Instance.UIManager.Get<PlayerBtn>().skillPanel; // 스킬 버튼 UI
        dashGame = game;

        // 대사 완료 이벤트 등록
        Managers.Instance.DialogueManager.OnDialogStepEnd += CheckDialogueCompletion;
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        Managers.Instance.DialogueManager.OnDialogStepEnd -= CheckDialogueCompletion;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Managers.Instance.GameManager.ChapterProgress == 2)
        {
            skillBTN.ShowInteractionButton(true); // 버튼 표시
            skillBTN.OnInteractBtnClick += OnPlayerInteract;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            skillBTN.ShowInteractionButton(false); // 버튼 숨김
            skillBTN.OnInteractBtnClick -= OnPlayerInteract;
        }
        else return;
    }

    private void OnPlayerInteract()
    {
        if (interactionType == InteractionType.EndGame)
        {
            dashGame.EndGame(npcType); // NPC 정보를 함께 전달
        }
    }

    private void CheckDialogueCompletion(int completedDialogIndex)
    {
        // 다이얼로그 데이터와 SO파일 데이터 합쳐서 다시 리펙토링 필요
        if (completedDialogIndex == teleport)
        {
            PlayerTeleport(); // 플레이어 위치 변경
        }
        else if (completedDialogIndex == startGame)
        {
            EditorLog.Log("30007번 대사가 완료되었습니다.");
            dashGame.StartGame();
        }
    }

    private void PlayerTeleport()
    {
        var player = Managers.Instance.GameManager.Player;
        if (player != null)
        {
            player.transform.position = teleportPosition;
        }
    }
}
