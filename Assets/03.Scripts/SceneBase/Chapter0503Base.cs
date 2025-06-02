using System;
using UnityEngine;

public class Chapter0503Base : SceneBase
{
    [Header("Chapter 0503")]
    public GameObject crowd;

    public Collider2D meetingWomanTriger; // 앞
    public Collider2D meetingBihyiTriger; // 뒤
    
    private bool IsChapterProgress = false; // 챕터 진행 여부

    [SerializeField] private BgmLayeredFader bgmLayeredFader;

    private void Start()
    {
        var gm = Managers.Instance.GameManager;

        if (gm.SavePoint == 0)
        {
            // 처음 503 진입
            gm.ChapterProgress = 3;
            meetingWomanTriger.enabled = true;
            meetingBihyiTriger.enabled = false;
            gm.SavePoint++;
        }
        else if (gm.SavePoint == 1)
        {
            gm.ChapterProgress = 4; // 503 진입 시 ChapterProgress를 4로 설정
            // 두 번째 503 진입 (504 -> 503)
            meetingWomanTriger.enabled = false;
            meetingBihyiTriger.enabled = true;

            if (IsChapterProgress == true)
            {
                gm.UpdateProgress();
            }
        }
        else if (gm.SavePoint == 2)
        {
            // 세 번째 이상 503 진입
            gm.ChapterProgress = 5; // 503 진입 시 ChapterProgress를 4로 설정

            meetingWomanTriger.enabled = false;
            meetingBihyiTriger.enabled = true;
        }


        // Crowd 처리
        if (gm.ChapterProgress == 4)
            crowd.SetActive(false);

        EditorLog.Log($"[Chapter503] 최종 VisitCount: {gm.SavePoint}");
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
            IsChapterProgress = true;

            var gm = Managers.Instance.GameManager;
            if (gm.SavePoint == 1)
                gm.SavePoint++;    

            var upgrade = Managers.Instance.GameManager;
            if (upgrade.ChapterProgress == 4)
            {
                upgrade.UpdateProgress();
            }
        }
    }
}