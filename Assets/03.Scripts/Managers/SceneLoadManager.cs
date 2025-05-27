using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SceneLoadManager : MonoBehaviour
{
    private const float fakeMinDuration = 3f;
    private const float fakeMaxDuration = 4f;
    public SceneType CurrentScene { get; private set; } = SceneType.Title;
    public SceneType NextSceneToLoad { get; private set; } = SceneType.Title;
    public bool IsSceneLoadComplete { get; set; }

    public void LoadScene(SceneType loadScene)
    {
        StartCoroutine(LoadSceneCoroutine(loadScene));
    }

    // 씬을 로드하는 코루틴
    private IEnumerator LoadSceneCoroutine(SceneType loadScene)
    {
#if UNITY_EDITOR
        if (Managers.Instance.LoadTestScene)
            loadScene = Managers.Instance.TestScene;
#endif

        IsSceneLoadComplete = false;
        Managers.Instance.OnSceneUnloaded();

        // 로딩 씬을 먼저 로드
        NextSceneToLoad = loadScene;
        yield return SceneManager.LoadSceneAsync(SceneType.Loading.GetName(), LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneType.Loading.GetName()));

        yield return SceneManager.UnloadSceneAsync(CurrentScene.GetName());

        // 씬 로드 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadScene.GetName(), LoadSceneMode.Additive);
        if (operation == null)
        {
            EditorLog.LogError("SceneLoader.LoadSceneAsync: operation is null");
            yield break;
        }

        operation.allowSceneActivation = false;

        // 최소 로딩 시간을 보장하기 위해 가짜 로딩 시간을 설정
        float minDuration = Random.Range(fakeMinDuration, fakeMaxDuration);
        float fakeLoadTime = 0f;
        float progress = 0f;

        // 씬이 90% 로드될 때까지 로딩바를 채움
        while (progress < 0.9f)
        {
            fakeLoadTime += Time.deltaTime;
            var fakeLoadRatio = fakeLoadTime / minDuration;

            progress = Mathf.Min(operation.progress, fakeLoadRatio - 0.1f);
            yield return null;
        }

        // 로드 씬 활성화
        CurrentScene = loadScene;
        Managers.Instance.GameManager.SetCurrentScene(CurrentScene);
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;

        // 실제 씬 전환 완료 이후 초기화 및 로딩 시간 병렬 대기
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => IsSceneLoadComplete);
        SceneManager.UnloadSceneAsync(SceneType.Loading.GetName());
    }

    public void DebugMode()
    {
        CurrentScene = SceneType.Chapter1;
    }
}