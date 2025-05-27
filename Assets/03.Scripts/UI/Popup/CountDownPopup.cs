using System.Collections;
using TMPro;
using UnityEngine;

public class CountDownPopup : UIBase
{
    public int countDownTime = 5; // 카운트 다운 시간 (초)
    public TextMeshProUGUI countdownText;

    public void CountDownStart()
    {
        StartCoroutine(CountDownCoroutine());
    }

    IEnumerator CountDownCoroutine()
    {
        for (int i = countDownTime; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "Start!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false); // 카운트다운이 끝나면 텍스트 숨기기
    }
}
