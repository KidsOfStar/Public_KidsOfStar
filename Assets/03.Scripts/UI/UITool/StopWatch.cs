using TMPro;
using UnityEngine;

public class StopWatch : UIBase
{
    [Header("StopWatch")] // 스탑워치
    public float timeStart; // 시작 시간
    public float targetTime; // 목표 시간
    public float recodeTime; // 저장된 시간
    private bool isTiming = false; // 타이밍 여부

    public TextMeshProUGUI timeText; // UI 텍스트


    // Start is called before the first frame update
    void Start()
    {
        timeText.text = "00:00:00"; // 초기화
    }

    // Update is called once per frame
    void Update()
    {
        StartTime();
    }

    public void StartTime()
    {
        if(!isTiming) return; // 타이밍이 아닐 경우 종료
        timeStart += Time.deltaTime; // 시간 증가
        int minutes = (int)(timeStart / 60); // 분 계산
        int seconds = (int)(timeStart % 60); // 초 계산
        int milliseconds = (int)((timeStart - (int)timeStart) * 100); // 밀리초 계산

        timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds); // 텍스트 업데이트
    }

    public void OnStartWatch()
    {
        // 스탑워치 시동
        isTiming = true; // 타이밍 시작
    }

    public float OnStopWatch()
    {
        // 스탑워치 시간 정지
        isTiming = false; // 타이밍 종료
        recodeTime = timeStart; // 저장된 시간
        return recodeTime; // 저장된 시간 반환
    }

    public void ResetTime() // 시간 초기화 메소드
    {
        timeStart = 0; // 시간 초기화
        timeText.text = "00:00:00"; // UI 텍스트 초기화
    }

    //public void CheckTargetTime()
    //{
    //    // 도착했는데 목표 시간보다 늦을 경우 다시하기
    //    if (timeStart >= targetTime)
    //    {
    //        ResetTime(); // 시간 초기화 메소드 호출
    //    }
    //    // 목표 시간에 빨리 들어왔을 경우
    //    else if(timeStart <= targetTime)
    //    {
    //        timeStart = 0; // 시간 초기화
    //        timeText.text = "00:00:00"; // UI 텍스트 초기화
    //    }
    //}
}
