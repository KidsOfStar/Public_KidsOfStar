using System;
using UnityEngine;

public class Chapter0503Base : SceneBase
{
    [Header("Chapter 0503")]
    public GameObject crowd;

    public Collider2D meetingWomanTriger; // 앞
    public Collider2D meetingBihyiTriger; // 뒤

    [SerializeField] private BgmLayeredFader bgmLayeredFader;

    private void Start()
    {
        var gm = Managers.Instance.GameManager;

        if (gm.VisitCount == 0)
        {
            // 처음 503 진입
            gm.ChapterProgress = 3;
            meetingWomanTriger.enabled = true;
            meetingBihyiTriger.enabled = false;
            gm.VisitCount++;
        }
        else if (gm.VisitCount == 1)
        {
            // 두 번째 503 진입 (504 -> 503)
            meetingWomanTriger.enabled = false;
            meetingBihyiTriger.enabled = true;

            if (Managers.Instance.GameManager.ChapterProgress == 3)
                Managers.Instance.GameManager.UpdateProgress();
            
            if (Managers.Instance.GameManager.IsNewGame)
                gm.ChapterProgress = 4;
        }

        // Crowd 처리
        if (gm.ChapterProgress == 4)
            crowd.SetActive(false);

        EditorLog.Log($"[Chapter503] 최종 VisitCount: {gm.VisitCount}");
        EditorLog.Log(Managers.Instance.GameManager.ChapterProgress);
    }

    protected override void CreatePool()
    {
        base.CreatePool();
        Managers.Instance.PoolManager.CreatePool(bgmLayeredFader.gameObject, 1);
    }

    protected override void CutSceneEndCallback() { }

    protected override void InitSceneExtra(Action callback)
    {
        Managers.Instance.SoundManager.PlayBgm(BgmSoundType.Aquarium);
        Managers.Instance.SoundManager.PlayAmbience(AmbienceSoundType.Aquarium);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var upgrade = Managers.Instance.GameManager;
            if (upgrade.ChapterProgress == 4)
            {
                upgrade.UpdateProgress();
            }
        }
    }
}