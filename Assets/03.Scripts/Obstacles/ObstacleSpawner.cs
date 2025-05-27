using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclesSpawner : MonoBehaviour
{
#if TEST_BUILD
    [Header("Debug")]
    [SerializeField] private bool isTest = false;
#endif
    
    [Header("Settings")]
    [Tooltip("카메라 우측 떨어진 스폰 X값 위치")] 
    public float spawnXOffset = 10f;
    //고정된 Y값
    private float fixedPosY = -2.75f;

    [Tooltip("장애물 사이의 기본 고정 x 간격")]
    public float baseSpacing = 4f;

    [Tooltip("장애물 간 추가 랜덤 간격 최소값")]
    public float extraMin = 1f;

    [Tooltip("장애물 간 추가 랜덤 간격 최대값")]
    public float extraMax = 3f;


    // 장애물 생성 확률
    private float stoneProbability = 0.3f;
    private float smallSeaweedProbability = 0.2f;
    private float mediumSeaweedProbability = 0.35f;

    [Tooltip("장애물 목록 Pool 생성용")]
    public List<GameObject> obstaclePrefabs;
    // 마지막 장애물 스폰 X값
    private float lastSpawnX;

    [Header("Wave")]
    [Tooltip("Wave에 생성할 장애물의 개수")]
    public int waveObstacleCount = 8;
    //Wave에서 남아있는 장애물의 갯수
    private int currentWaveRemaining;
    // 현재 Wave
    private int currentWave = 1;     

    [Header("Dialogue Index")]
    [SerializeField] private int[] indexes;

    private int currentIndex;
    private readonly WaitForSeconds dialogEndTime = new(4f);
    private bool isGameOver = false;

    //장애물을 Pool에서 스폰
    public void StartSpawn()
    {
#if TEST_BUILD
        if (isTest)
        {
            currentWave = 3;
            currentWaveRemaining = waveObstacleCount - 1;
        }
#endif

        foreach (GameObject prefab in obstaclePrefabs)
        {
            Managers.Instance.PoolManager.CreatePool(prefab, 10);
        }

        SpawnWave();
    }

    // 웨이브에 따른 장애물 스폰
    private void SpawnWave()
    {
        // 카메라의 우측 화면을 기준으로 좌표 계산
        var mainCam = Managers.Instance.GameManager.MainCamera;
        float screenRight = mainCam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        lastSpawnX = screenRight + spawnXOffset;

        // 남은 장애물의 수 초기화
        currentWaveRemaining = waveObstacleCount;

        for (int i = 0; i < waveObstacleCount; i++)
        {
            SpawnNextObstacle();
        }
    }

    // Poolkey 매핑
    private string GetPoolKey(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.SmallSeaweed:
                return "SmallSeaweed";
            case ObstacleType.MediumSeaweed:
                return "MediumSeaweed";
            case ObstacleType.LargeSeaweed:
                return "LargeSeaweed";
            case ObstacleType.Stone:
                return "Stone";
            default:
                return "";
        }
    }

    // 장애물 종류를 결정
    private ObstacleType ChooseRandomTypeSpawner()
    {
        float rand = Random.value;
        if (rand < stoneProbability)
        {
            return ObstacleType.Stone;
        }

        float seaweedRand = Random.value;
        if (seaweedRand < smallSeaweedProbability)
        {
            return ObstacleType.SmallSeaweed;
        }
        else if (seaweedRand < smallSeaweedProbability + mediumSeaweedProbability)
        {
            return ObstacleType.MediumSeaweed;
        }
        else
        {
            return ObstacleType.LargeSeaweed;
        }
    }

    // 스폰 위치 계산
    private Vector3 GetSpawnPosition()
    {
        float spacing = baseSpacing + Random.Range(extraMin, extraMax);
        lastSpawnX += spacing;
        return new Vector3(lastSpawnX, fixedPosY, 0f);
    }

    // 실제 장애물 스폰
    private void SpawnNextObstacle()
    {
        if (isGameOver) return;
        
        ObstacleType chosenType = ChooseRandomTypeSpawner();
        string poolKey = GetPoolKey(chosenType);

        GameObject obj = Managers.Instance.PoolManager.Spawn(poolKey, Vector3.zero, Quaternion.identity);
        Obstacle obstacle = obj.GetComponent<Obstacle>();
        if (obstacle)
        {
            Vector3 spawnPos = GetSpawnPosition();
            obstacle.InitObstacle(spawnPos, chosenType);
        }
    }

    // 실제 장애물 디스폰
    public void OnObstacleDespawned()
    {
        if (isGameOver) return;
        
        currentWaveRemaining--;

        // 한 웨이브가 끝났다면
        if (currentWaveRemaining > 0) return;

        if (currentWave >= 3)
        {
            RecordChaseClear();
            Managers.Instance.DialogueManager.OnDialogEnd -= SpawnWave;
            Managers.Instance.SoundManager.StopBgm();
            Managers.Instance.CutSceneManager.PlayCutScene(CutSceneType.Rescued);
        }
        else
        {
            currentWave++;

            // 대사 끝났음 이벤트에 스폰 웨이브 구독
            Managers.Instance.DialogueManager.OnDialogEnd -= SpawnWave;
            Managers.Instance.DialogueManager.OnDialogEnd += SpawnWave;

            // 대사 출력
            Managers.Instance.DialogueManager.SetCurrentDialogData(indexes[currentIndex]);
            currentIndex++;

            StartCoroutine(OnDialogEnd());
        }
    }

    private void RecordChaseClear()
    {
        var analytics = Managers.Instance.AnalyticsManager;
        analytics.RecordChapterEvent("ChaseClear",
                                     ("ChallengeCount", analytics.TryCount));

        analytics.TryCount = 0;
    }

    private IEnumerator OnDialogEnd()
    {
        yield return dialogEndTime;

        Managers.Instance.DialogueManager.OnClick?.Invoke();
    }

    public void GameOver()
    {
        isGameOver = true;
        StopAllCoroutines();
        Managers.Instance.DialogueManager.OnDialogEnd -= SpawnWave;
        Destroy(Managers.Instance.GameManager.Player.gameObject);
    }

    private void OnDestroy()
    {
        Managers.Instance.DialogueManager.OnDialogEnd -= SpawnWave;
    }
}